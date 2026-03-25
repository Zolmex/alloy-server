using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Common.Network;

// Connection to other server apps
public class AppConnection
{
    private const int RESPONSE_TTL_MS = 5000;
    private static readonly Logger _log = new(typeof(AppConnection));

    private readonly SocketAsyncEventArgs _receiveSAEA;
    private readonly SocketReceiveState _receiveState;
    private readonly SocketAsyncEventArgs _sendSAEA;
    private readonly SocketSendState _sendState;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<IAppMessageAck>> _pendingAcks;
    private readonly Channel<IAppMessage> _sendChannel = Channel.CreateUnbounded<IAppMessage>(
        new UnboundedChannelOptions { SingleReader = true, AllowSynchronousContinuations = false });
    
    private readonly SemaphoreSlim _sendSignal = new(0);
    
    public Socket Socket { get; private set; }
    public string HostName { get; set; }
    public string TargetName { get; set; }

    public AppConnection()
    {
        ThreadPool.SetMinThreads(2000, 2000);
        
        _sendState = new SocketSendState();
        _receiveState = new SocketReceiveState();
        _pendingAcks = new ConcurrentDictionary<int, TaskCompletionSource<IAppMessageAck>>();

        _sendSAEA = new SocketAsyncEventArgs();
        _sendSAEA.Completed += ProcessSend;

        _receiveSAEA = new SocketAsyncEventArgs();
        _receiveSAEA.Completed += ProcessReceive;
    }

    private void Dispose()
    {
        _sendState.Dispose();
        _receiveState.Dispose();
    }

    public void Setup(Socket socket) { Socket = socket; Socket.NoDelay = true; }
    public void SetupNew() { Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); Socket.NoDelay = true; }

    public async Task Connect(string host, int port)
    {
        await Socket.ConnectAsync(host, port);
        Start(Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown");
    }

    public void Start(string hostName)
    {
        HostName = hostName;
        Send(new HelloMessage() { AppName = hostName });
        
        // Start processing loops
        Task.Run(ReceiveLoop);
        Task.Run(SendLoop);
    }

    public void Send(IAppMessage msg)
    {
        _sendChannel.Writer.TryWrite(msg);
    }

    public async Task<T> SendAndReceiveAsync<T>(IAppMessage msg) where T : IAppMessageAck
    {
        IAppMessage.SetSequence(msg);
        var tcs = new TaskCompletionSource<IAppMessageAck>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingAcks[msg.Sequence] = tcs;

        Send(msg);

        using var cts = new CancellationTokenSource(RESPONSE_TTL_MS);
        try
        {
            await using (cts.Token.Register(() => tcs.TrySetCanceled()))
            {
                return (T)await tcs.Task;
            }
        }
        catch (OperationCanceledException)
        {
            _pendingAcks.TryRemove(msg.Sequence, out _);
            _log.Error($"Response for {msg.MessageId} (Seq:{msg.Sequence}) timed out");
            return default;
        }
        finally
        {
            _pendingAcks.TryRemove(msg.Sequence, out _);
        }
    }

    private async Task SendLoop()
    {
        // Local buffer to stage outgoing data without fighting locks
        while (await _sendChannel.Reader.WaitToReadAsync())
        {
            // Reset state for a new batch
            _sendState.Reset();

            // Drain the channel into the buffer to send multiple messages in one TCP packet
            while (_sendChannel.Reader.TryRead(out var msg))
            {
                _sendState.WriteMessage(msg);
                // If buffer is 75% full, send it now to avoid overflow
                if (_sendState.LastValidIndex > 48000) break; 
            }

            if (_sendState.HasData)
            {
                _sendState.PrepareSAEA(_sendSAEA);
                
                if (!Socket.SendAsync(_sendSAEA))
                    ProcessSend(null, _sendSAEA);
            }
        }
    }

    private void ProcessSend(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError != SocketError.Success)
        {
            Disconnect($"Send Error: {args.SocketError}");
            return;
        }

        lock (_sendState)
            _sendState.Reset();
        
        // Check if more messages were added while we were sending
        if (_sendState.HasData) _sendSignal.Release();
    }

    private void ReceiveLoop()
    {
        if (Socket == null || !Socket.Connected) return;
        _receiveState.PrepareSAEA(_receiveSAEA);
        
        if (!Socket.ReceiveAsync(_receiveSAEA))
            ProcessReceive(null, _receiveSAEA);
    }

    private void ProcessReceive(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
        {
            Disconnect("Receive Error or Connection Closed");
            return;
        }

        _receiveState.OnDataReceived(args.BytesTransferred);

        while (_receiveState.TryReadMessage(out var msg))
        {
            if (msg.IsAck && _pendingAcks.TryRemove(msg.Sequence, out var tcs))
                tcs.TrySetResult(msg as IAppMessageAck);

            try
            {
                _ = msg.HandleAsync(this);
            }
            catch (Exception ex)
            {
                _log.Error($"Error handling message {msg.MessageId}: {ex.Message}");
            }
        }

        ReceiveLoop();
    }

    public void Disconnect(string reason = "Shutdown")
    {
        if (Socket is { Connected: true })
        {
            _log.Info($"Disconnected from app '{TargetName}'. Reason: {reason}");
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }
        Dispose();
    }
}