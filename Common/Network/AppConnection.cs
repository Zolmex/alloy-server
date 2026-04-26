using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;

namespace Common.Network;

// Connection to other server apps
public class AppConnection {
    private const int RESPONSE_TTL_MS = 20000;
    private static readonly Logger _log = new(typeof(AppConnection));
    private readonly ConcurrentDictionary<int, TaskCompletionSource<IAppMessageAck>> _pendingAcks;

    private readonly SocketAsyncEventArgs _receiveSAEA;
    private readonly SocketReceiveState _receiveState;

    private readonly Channel<IAppMessage> _sendChannel = Channel.CreateUnbounded<IAppMessage>(
        new UnboundedChannelOptions { SingleReader = true, AllowSynchronousContinuations = false });

    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly SocketAsyncEventArgs _sendSAEA;
    private readonly SocketSendState _sendState;

    public AppConnection() {
        _sendState = new SocketSendState();
        _receiveState = new SocketReceiveState(0x40000);
        _pendingAcks = new ConcurrentDictionary<int, TaskCompletionSource<IAppMessageAck>>();

        _sendSAEA = new SocketAsyncEventArgs();
        _sendSAEA.Completed += ProcessSend;

        _receiveSAEA = new SocketAsyncEventArgs();
        _receiveSAEA.Completed += async (sender, args) => await ProcessReceive(sender, args);
    }

    public Socket Socket { get; private set; }
    public string HostName { get; set; }
    public string TargetName { get; set; }

    private void Dispose() {
        _sendState.Dispose();
        _receiveState.Dispose();
    }

    public void Setup(Socket socket) {
        Socket = socket;
        Socket.NoDelay = true;
    }

    public void SetupNew() {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket.NoDelay = true;
    }

    public async Task Connect(string host, int port) {
        await Socket.ConnectAsync(host, port);
        Start(Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown");
    }

    public void Start(string hostName) {
        HostName = hostName;
        Send(new HelloMessage { AppName = hostName });

        // Start processing loops
        Task.Run(ReceiveLoop);
        Task.Run(SendLoop);
    }

    public void Send(IAppMessage msg) {
        _sendChannel.Writer.TryWrite(msg);
    }

    public async Task<T> SendAndReceiveAsync<T>(IAppMessage msg) where T : IAppMessageAck {
        IAppMessage.SetSequence(msg);
        var tcs = new TaskCompletionSource<IAppMessageAck>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingAcks[msg.Sequence] = tcs;

        Send(msg);

        try {
            return (T)await tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(RESPONSE_TTL_MS));
        }
        catch (TimeoutException) {
            _log.Error($"Response for {msg.MessageId} (Seq:{msg.Sequence}) timed out");
            return default;
        }
        finally {
            _pendingAcks.TryRemove(msg.Sequence, out _);
        }
    }

    private async Task SendLoop() {
        // Local buffer to stage outgoing data without fighting locks
        while (await _sendChannel.Reader.WaitToReadAsync()) {
            await _sendLock.WaitAsync();

            // Drain the channel into the buffer to send multiple messages in one TCP packet
            while (_sendChannel.Reader.TryRead(out var msg))
                if (!_sendState.WriteMessage(msg))
                    break;

            using (TimedLock.Lock(_sendState)) {
                if (_sendState.TryBeginSend(_sendSAEA)) {
                    if (!Socket.SendAsync(_sendSAEA))
                        ProcessSend(null, _sendSAEA);
                }
                else {
                    _sendLock.Release();
                }
            }
        }
    }

    private void ProcessSend(object sender, SocketAsyncEventArgs args) {
        while (true) {
            if (args.SocketError != SocketError.Success) {
                Disconnect($"Send Error: {args.SocketError}");
                return;
            }

            using (TimedLock.Lock(_sendState)) {
                if (_sendState.OnDataSent(args)) {
                    if (!Socket.SendAsync(_sendSAEA))
                        continue;
                    return;
                }
            }

            _sendLock.Release();
            break;
        }
    }

    private async Task ReceiveLoop() {
        while (Socket != null && Socket.Connected) {
            _receiveState.PrepareSAEA(_receiveSAEA);

            if (Socket.ReceiveAsync(_receiveSAEA))
                break;

            if (!await HandleReceive(_receiveSAEA))
                break;
        }
    }

    private async Task ProcessReceive(object sender, SocketAsyncEventArgs args) {
        if (await HandleReceive(args))
            ReceiveLoop();
    }

    private async Task<bool> HandleReceive(SocketAsyncEventArgs args) {
        if (args.SocketError != SocketError.Success || args.BytesTransferred == 0) {
            Disconnect("Receive Error or Connection Closed");
            return false;
        }

        _receiveState.OnDataReceived(args.BytesTransferred);

        while (_receiveState.TryReadMessage(out var msg)) {
            if (msg.IsAck && _pendingAcks.TryRemove(msg.Sequence, out var tcs))
                tcs.TrySetResult(msg as IAppMessageAck);

            try {
                await msg.HandleAsync(this);
            }
            catch (Exception ex) {
                _log.Error($"Error handling message {msg.MessageId}: {ex.Message}");
            }
        }

        return true;
    }

    public void Disconnect(string reason = "Shutdown") {
        if (Socket is { Connected: true }) {
            _log.Info($"Disconnected from app '{TargetName}'. Reason: {reason}");
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }

        Dispose();
    }
}