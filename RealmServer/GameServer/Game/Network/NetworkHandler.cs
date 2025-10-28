#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

#endregion

namespace GameServer.Game.Network
{
    public class SocketSendState
    {
        public NetworkWriter Writer { get; private set; }
        public MemoryStream Stream { get; private set; }
        public byte[] SocketBuffer { get; private set; } = new byte[NetworkHandler.SEND_BUFFER_SIZE];
        public int LastValidIndex { get; set; } // End position of the packet that still fits within buffer size
        public int Offset { get; set; }

        public SocketSendState()
        {
            Stream = new MemoryStream();
            Writer = new NetworkWriter(Stream);
        }

        public int
            PacketBegin() // Returns the start of the packet bytes in the buffer. NEVER use this without locking the SocketSendState instance
        {
            var begin = (int)Stream.Position;
            Stream.Position += 5; // Leave 5 bytes for the header
            return begin;
        }

        public void PacketEnd(int begin, PacketId pkt)
        {
            var length = (int)Stream.Position - begin;
            Stream.Position -= length; // Write the header [length][id]
            Writer.Write(length);
            Writer.Write((byte)pkt);
            Stream.Position += length - 5; // Go to the next position after packet body to write the next packet

            if (Stream.Position - Offset < NetworkHandler.SEND_BUFFER_SIZE)
                LastValidIndex = (int)Stream.Position;

            // Logger.Debug($"WRITING {pkt} DATA TO BUFFER");
        }

        public void Reset()
        {
            using (TimedLock.Lock(this))
            {
                LastValidIndex = 0;
                Offset = 0;
                Stream.Position = 0;
            }
        }
    }

    // Handles all of the network socket communication
    public class NetworkHandler
    {
        public const int SEND_BUFFER_SIZE = 0x40000; // 256KB
        public const int RECV_BUFFER_SIZE = 0x10000; // 64KB

        private static readonly Logger _log = new(typeof(NetworkHandler));

        public string IP { get; private set; }
        public User User { get; private set; }
        public Socket Socket { get; private set; }
        public SocketSendState SendState { get; private set; }

        private readonly Dictionary<PacketId, IIncomingPacket> _incomingPackets = PacketLib.LoadIncoming();

        private readonly SocketAsyncEventArgs _send;
        private readonly SocketAsyncEventArgs _receive;

        private readonly NetworkReader _reader;
        private readonly byte[] _readBuffer;
        private int _bytesRead;
        private bool _pendingSend;

        public NetworkHandler(User user)
        {
            User = user;

            _receive = new SocketAsyncEventArgs();
            _send = new SocketAsyncEventArgs();
            SendState = new SocketSendState();

            _readBuffer = new byte[RECV_BUFFER_SIZE];
            _reader = new NetworkReader(new MemoryStream(_readBuffer));

            // Setup our SAEA objects
            _receive.SetBuffer(_readBuffer, 0, RECV_BUFFER_SIZE);
            _receive.Completed += ProcessReceive;
            _send.SetBuffer(SendState.SocketBuffer, 0, SEND_BUFFER_SIZE);
            _send.Completed += ProcessSend;
        }

        // Reset this instance's values for a possible future connection
        public void Reset()
        {
            IP = null;

            _bytesRead = 0;
            _pendingSend = false;
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            SendState.Reset();
        }

        public void Setup(string ip, Socket socket)
        {
            IP = ip;
            Socket = socket;
            Socket.NoDelay = true;
        }

        public void SendSocketData()
        {
            if (_pendingSend)
                return;

            int count;
            using (TimedLock.Lock(SendState))
            {
                if (SendState.LastValidIndex == 0)
                    return;

                count = SendState.LastValidIndex - SendState.Offset;
                if (count == 0)
                    return;

                var streamBuffer = SendState.Stream.GetBuffer();
                if (SendState.SocketBuffer.Length < SendState.Offset + count)
                {
                    _log.Error($"Output buffer is too small: Stream: {streamBuffer.Length}, Offset: {SendState.Offset}, Count: {count}");
                    return;
                }

                Buffer.BlockCopy(streamBuffer, SendState.Offset, SendState.SocketBuffer, 0, count);

                var pos = (int)SendState.Stream.Position;
                if (pos == SendState.LastValidIndex) // This means that everything in the stream was written to the output buffer
                {
                    SendState.Stream.Position = 0;
                    SendState.LastValidIndex = 0;
                    SendState.Offset = 0;
                }
                else
                {
                    SendState.Offset = SendState.LastValidIndex;
                    SendState.LastValidIndex = pos;
                }
            }

            _pendingSend = true;
            _send.SetBuffer(0, count);

            try
            {
                if (Socket != null && !Socket.SendAsync(_send))
                    ProcessSend(null, _send);
            }
            catch
            { }
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs args)
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            var error = args.SocketError;
            if (error != SocketError.Success && error != SocketError.IOPending)
            {
                string msg = null;
                if (error != SocketError.ConnectionReset)
                    msg = $"Send SocketError.{error}";
                User.Disconnect(msg, DisconnectReason.NetworkError);
                return;
            }

            _pendingSend = false;
        }

        public void StartReceive()
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            // Some exceptions from the SAEA.SetBuffer method and from the Socket.ReceiveAsync() method
            // can't be avoided in certain situations, so a try-catch block is required.
            try
            {
                _receive.SetBuffer(_bytesRead, RECV_BUFFER_SIZE - _bytesRead);

                if (!Socket.ReceiveAsync(_receive))
                    ProcessReceive(null, _receive);
            }
            catch
            { }
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs args)
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
            {
                User.Disconnect(reason: DisconnectReason.Unknown);
                return;
            }

            var error = args.SocketError;

            // Check for any errors during the operation
            if (error != SocketError.Success && error != SocketError.IOPending)
            {
                string msg = null;
                if (error != SocketError.ConnectionReset)
                    msg = $"Receive SocketError.{error}";
                User.Disconnect(msg, DisconnectReason.NetworkError);
                return;
            }

            var bytesReceived = args.BytesTransferred;

            // When using ReceiveAsync, this value is set to 0 when the connection is terminated by the user
            if (bytesReceived == 0)
            {
                User.Disconnect(reason: DisconnectReason.UserDisconnect);
                return;
            }

            // Start reading a new packet
            var bytesNotRead = bytesReceived;
            while (bytesNotRead > 0)
            {
                var start = (int)_reader.BaseStream.Position;

                var length = _reader.ReadInt32(); // We always start reading from the beginning
                // Log.Debug($"Start: {start} Length: {length} BytesNotRead: {bytesNotRead} BytesRead: {_bytesRead}");

                // Policy file check
                if (length == 1014001516)
                {
                    SendPolicyFile();
                    break;
                }

                var read = Math.Min(length - _bytesRead, bytesNotRead); // Bytes read in this current iteration
                _bytesRead += read;

                if (_bytesRead < length) // This means we still have bytes to read that we haven't received in this call
                {
                    Buffer.BlockCopy(_readBuffer, start, _readBuffer, 0,
                        _bytesRead); // Move this many bytes to the beginning of the buffer
                    break;
                }

                var packetId = (PacketId)_reader.ReadByte();
                if (_incomingPackets.TryGetValue(packetId, out var pkt))
                {
                    // _log.Debug($"RECEIVING {packetId} ({read} bytes)");
                    try
                    {
                        pkt.Read(_reader);
                        pkt.Handle(User);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e);
                        User.SendFailure(Failure.DEFAULT, $"Error handling packet {packetId}: {e.Message}");
                    }
                }

                _reader.BaseStream.Seek(start + _bytesRead,
                    SeekOrigin.Begin); // In case we failed processing, go to the end of the packet
                _bytesRead = 0;

                bytesNotRead -= read;
            }

            _reader.BaseStream.Seek(0, SeekOrigin.Begin);

            StartReceive();
        }

        private void SendPolicyFile()
        {
            if (User.State == ConnectionState.Disconnected || !Socket.Connected)
                return;

            try
            {
                var wtr = new NetworkWriter(new NetworkStream(Socket));
                wtr.WriteNullTerminatedString(
                    @"<cross-domain-policy>" +
                    @"<allow-access-from domain=""*"" to-ports=""*"" />" +
                    @"</cross-domain-policy>");
                wtr.Write((byte)'\r');
                wtr.Write((byte)'\n');
                wtr.Close();
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
            }
        }
    }
}