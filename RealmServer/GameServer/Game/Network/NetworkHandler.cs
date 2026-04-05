#region

using Common.Network;
using Common.Network.Messaging;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#endregion

namespace GameServer.Game.Network;

// Handles all of the network socket communication
public class NetworkHandler
{
    public const int SEND_BUFFER_SIZE = 0x40000; // 256KB
    public const int RECV_BUFFER_SIZE = 0x10000; // 64KB

    private static readonly Logger _log = new(typeof(NetworkHandler));

    private readonly Dictionary<PacketId, IIncomingPacket> _incomingPackets = PacketLib.LoadIncoming();

    private readonly SocketAsyncEventArgs _receiveSAEA;
    private readonly SocketReceiveState _receiveState;
    private readonly SocketAsyncEventArgs _sendSAEA;
    private readonly SocketSendState _sendState;
    
    public NetworkHandler(User user)
    {
        User = user;

        _sendState = new SocketSendState();
        _receiveState = new SocketReceiveState();

        _sendSAEA = new SocketAsyncEventArgs();
        _sendSAEA.Completed += ProcessSend;

        _receiveSAEA = new SocketAsyncEventArgs();
        _receiveSAEA.Completed += ProcessReceive;
    }

    public string IP { get; private set; }
    public User User { get; }
    public Socket Socket { get; private set; }

    // Reset this instance's values for a possible future connection
    public void Reset()
    {
        IP = null;

        _sendState.Reset();
        _receiveState.Reset();
    }

    public void Setup(string ip, Socket socket)
    {
        IP = ip;
        Socket = socket;
        Socket.NoDelay = true;
    }

    public void WritePacket(IOutgoingPacket packet)
    {
        // Console.WriteLine($"SENDING {packet.ID}");
        _sendState.WritePacket(packet, (byte)packet.ID);
    }

    public void SendSocketData()
    {
        if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            return;

        if (!_sendState.TryBeginSend(_sendSAEA))
            return;

        if (!Socket.SendAsync(_sendSAEA))
            ProcessSend(null, _sendSAEA);
    }

    private void ProcessSend(object sender, SocketAsyncEventArgs args)
    {
        while (true)
        {
            if (User.State == ConnectionState.Disconnected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                break;
            }

            if (args.SocketError != SocketError.Success)
            {
                User.Disconnect($"Send Error: {args.SocketError}", DisconnectReason.NetworkError);
                break;
            }

            if (_sendState.OnDataSent(args))
                if (Socket.SendAsync(_sendSAEA))
                    break;

            break;
        }
    }

    public void StartReceive()
    {
        Task.Run(ReceiveLoop);
    }
    
    private void ReceiveLoop()
    {
        while (User.State != ConnectionState.Disconnected && Socket != null && Socket.Connected)
        {
            if (Socket == null || !Socket.Connected)
                return;

            _receiveState.PrepareSAEA(_receiveSAEA);

            if (Socket.ReceiveAsync(_receiveSAEA)) // Completed synchronously
                break;

            if (!HandleReceive(_receiveSAEA))
                break;
        }
    }

    private void ProcessReceive(object sender, SocketAsyncEventArgs args)
    {
        if (HandleReceive(args))
            ReceiveLoop();
    }

    private bool HandleReceive(SocketAsyncEventArgs args)
    {
        if (User.State == ConnectionState.Disconnected || args.BytesTransferred == 0)
        {
            User.Disconnect(reason: DisconnectReason.Unknown);
            return false;
        }

        var error = args.SocketError;

        // Check for any errors during the operation
        if (error != SocketError.Success && error != SocketError.IOPending)
        {
            string msg = null;
            if (error != SocketError.ConnectionReset)
                msg = $"Receive SocketError.{error}";
            User.Disconnect(msg, DisconnectReason.NetworkError);
            return false;
        }

        _receiveState.OnDataReceived(args.BytesTransferred);

        while (_receiveState.PacketReady())
        {
            var pktId = (PacketId)_receiveState.ReadPacket(out var rdr);
            try
            {
                // Console.WriteLine($"RECEIVING {pktId}");
                if (_incomingPackets.TryGetValue(pktId, out var pkt))
                {
                    pkt.Read(ref rdr);
                    pkt.Handle(User);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error handling message {pktId}: {ex.Message}");
            }
        }

        return true;
    }
}