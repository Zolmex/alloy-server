using Common.Network.Messaging;
using Common.Resources.Config;
using Common.Utilities;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common.Network;

// Connection to other server apps
public class AppConnection
{
    private static readonly Logger _log = new(typeof(AppConnection));
    private readonly SocketAsyncEventArgs _receiveSAEA;
    private readonly SocketReceiveState _receiveState;
    private readonly SocketAsyncEventArgs _sendSAEA;
    private readonly SocketSendState _sendState;

    private readonly Socket _socket;

    public AppConnection()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _sendState = new SocketSendState();
        _receiveState = new SocketReceiveState();

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

    public async Task Connect(DatabaseConfig config)
    {
        await _socket.ConnectAsync(config.Host, config.Port);

        StartReceive();
        _ = StartSend();
    }

    public void Send(IAppMessage msg)
    {
        _sendState.WriteMessage(msg);
    }

    private async Task StartSend()
    {
        if (_sendState.LastValidIndex == 0) // Empty buffer
        {
            await Task.Delay(10); // Wait 10 milliseconds to retry
            await StartSend();
        }

        _sendState.Lock(); // Block new write operations until the buffer is written to the socket

        if (!_socket.SendAsync(_sendSAEA))
            ProcessSend(null, _sendSAEA);
    }

    private void ProcessSend(object sender, SocketAsyncEventArgs args) // Handle errors
    {
        if (!_socket.Connected)
        {
            Disconnect("Unknown");
            return;
        }

        var error = args.SocketError;
        if (error is SocketError.Success or SocketError.IOPending) // Everything good
        {
            _sendState.Unlock(); // Buffer was successfully written to the socket, continue write operations
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
        if (!_socket.ReceiveAsync(_receiveSAEA))
            ProcessReceive(null, _receiveSAEA);
    }

    private void ProcessReceive(object sender, SocketAsyncEventArgs args)
    {
        if (!_socket.Connected)
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

        while (_receiveState.ReadMessage(ref bytesReceived, out var msg)) // Returns false when it fails to read a packet
        {
            msg.Read(); // Every message defines a read method and a write method
            msg.Handle(); // Handler is managed by each individual app
        }

        StartReceive();
    }

    public void Disconnect(string reason = "Shutdown")
    {
        _log.Info($"DbServer connection shut down. Reason: {reason}");
        _socket.Disconnect(false);
        _socket.Close();
    }
}