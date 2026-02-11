using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
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

    public Socket Socket { get; private set; }
    public string HostName { get; set; }
    public string TargetName { get; set; }

    public AppConnection()
    {
        _sendState = new SocketSendState();
        _receiveState = new SocketReceiveState();
        _pendingAcks = new ConcurrentDictionary<int, TaskCompletionSource<IAppMessageAck>>();

        _sendSAEA = new SocketAsyncEventArgs();
        _sendSAEA.SetBuffer(_sendState.Buffer, 0, _sendState.Buffer.Length);
        _sendSAEA.Completed += ProcessSend;

        _receiveSAEA = new SocketAsyncEventArgs();
        _receiveSAEA.SetBuffer(_receiveState.Buffer, 0, _receiveState.Buffer.Length);
        _receiveSAEA.Completed += ProcessReceive;
    }

    private void Reset()
    {
        _sendState.Reset();
        _receiveState.Reset();
    }

    public void Setup(Socket socket)
    {
        Socket = socket;
        Socket.NoDelay = true;
    }

    public void SetupNew()
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket.NoDelay = true;
    }

    public async Task Connect(string host, int port)
    {
        await Socket.ConnectAsync(host, port);

        Start(Assembly.GetEntryAssembly()!.GetName().Name);
    }

    public void Start(string hostName)
    {
        HostName = hostName;
        _log.Info($"Started a connection with the host name: {hostName}");
        Send(new HelloMessage() { AppName = hostName });

        Task.Run(StartReceive);
        _ = StartSend();
    }

    public void Send(IAppMessage msg)
    {
        _sendState.WriteMessage(msg);
    }

    public async Task<T> SendAndReceiveAsync<T>(IAppMessage msg) where T : IAppMessageAck // Sends packet and completes when it receives a response
    {
        IAppMessage.SetSequence(msg);

        Logger.Debug($"Sending async {msg.MessageId}:{msg.Sequence}");
        _sendState.WriteMessage(msg);

        var tcs = _pendingAcks[msg.Sequence] = new TaskCompletionSource<IAppMessageAck>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var cts = new CancellationTokenSource(RESPONSE_TTL_MS);
        try
        {
            await using (cts.Token.Register(() =>
                             tcs.TrySetException(new TimeoutException(
                                 $"Response for {msg.MessageId} (Seq:{msg.Sequence}) timed out"))))
            {
                return (T)await tcs.Task;
            }
        }
        catch (TimeoutException e)
        {
            _log.Error(e.Message);
            return default;
        }
    }

    private async Task StartSend()
    {
        while (_sendState.LastValidIndex == 0) // Empty buffer
            await Task.Delay(10); // Wait 10 milliseconds to retry

        _sendState.BeginSend(_sendSAEA); // Block new write operations until the buffer is written to the socket
        if (!Socket.SendAsync(_sendSAEA))
            ProcessSend(null, _sendSAEA);
    }

    private void ProcessSend(object sender, SocketAsyncEventArgs args) // Handle errors
    {
        if (!Socket.Connected)
        {
            Disconnect("Unknown");
            return;
        }

        var error = args.SocketError;
        if (error is SocketError.Success or SocketError.IOPending) // Everything good
        {
            _sendState.EndSend(); // Buffer was successfully written to the socket, continue write operations
            _ = StartSend();
            return;
        }

        string msg = null;
        if (error != SocketError.ConnectionReset)
            msg = $"Send SocketError.{error}";
        Disconnect(msg);
    }

    private void StartReceive()
    {
        _receiveState.SetBuffer(_receiveSAEA); // Prepare buffer for next batch of packets
        if (!Socket.ReceiveAsync(_receiveSAEA))
            ProcessReceive(null, _receiveSAEA);
    }

    private void ProcessReceive(object sender, SocketAsyncEventArgs args)
    {
        if (!Socket.Connected)
        {
            Disconnect("Unknown");
            return;
        }

        var error = args.SocketError;

        // Check for any errors during the operation
        if (error != SocketError.Success && error != SocketError.IOPending)
        {
            string msg = null;
            if (error != SocketError.ConnectionReset)
                msg = $"Receive SocketError.{error}";
            Disconnect(msg);
            return;
        }

        var bytesReceived = args.BytesTransferred;

        // When using ReceiveAsync, this value is set to 0 when the connection is terminated by the user
        if (bytesReceived == 0)
        {
            Disconnect();
            return;
        }

        Logger.Debug($"Received {bytesReceived} bytes");
        
        var bytesNotRead = bytesReceived;
        while (_receiveState.ReadMessage(ref bytesNotRead, out var msg)) // Returns false when it fails to read a packet
        {
            msg.Read(_receiveState.Reader); // Every message defines a read method and a write method
            if (msg.IsAck)
            {
                if (_pendingAcks.TryRemove(msg.Sequence, out var tcs))
                    tcs.TrySetResult(msg as IAppMessageAck); // Completes SendAsync execution
            }
            
            Logger.Debug($"Received {msg.MessageId}:{msg.Sequence}(IsAck?{msg.IsAck})");

            msg.Handle(this); // Handler is implemented by each individual app
        }

        _receiveState.Reset();
        StartReceive();
    }

    public void Disconnect(string reason = "Shutdown")
    {
        _log.Info($"Disconnected from app '{TargetName}'. Reason: {reason}");
        Socket.Disconnect(true);
        Reset();
    }
}