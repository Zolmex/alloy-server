using Common.Network.Messaging;
using Common.Utilities;
using System;
using System.IO;
using System.Net.Sockets;

namespace Common.Network;

public class SocketReceiveState
{
    public readonly NetworkReader Reader;

    public byte[] Buffer { get; } = new byte[0x10000]; // 64 KB
    private int _bytesRead;
    private int _lastStart;

    public SocketReceiveState()
    {
        Reader = new NetworkReader(new MemoryStream(Buffer));
    }
    
    public void Reset()
    {
        _bytesRead = 0;
        _lastStart = 0;
        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
    }

    public void SetBuffer(SocketAsyncEventArgs args)
    {
        args.SetBuffer(_bytesRead, Buffer.Length - _bytesRead);
    }

    public bool ReadMessage(ref int bytesNotRead, out IAppMessage msg)
    {
        // Start reading a new packet
        msg = null;
        if (bytesNotRead <= 0)
        {
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        if (_bytesRead != 0)
        {
            Reader.BaseStream.Seek(_lastStart + _bytesRead,
                SeekOrigin.Begin); // In case we failed processing, go to the end of the packet
            _bytesRead = 0;
        }
        
        _lastStart = (int)Reader.BaseStream.Position;

        var length = Reader.ReadInt32(); // We always start reading from the beginning
        // Log.Debug($"Start: {start} Length: {length} BytesNotRead: {bytesNotRead} BytesRead: {_bytesRead}");

        if (length == 0)
            throw new InvalidDataException("Message length cannot be zero.");

        var read = Math.Min(length - _bytesRead, bytesNotRead); // Bytes read in this current iteration
        _bytesRead += read;

        if (_bytesRead < length) // This means we still have bytes to read that we haven't received in this call
        {
            System.Buffer.BlockCopy(Buffer, _lastStart, Buffer, 0,
                _bytesRead); // Move this many bytes to the beginning of the buffer
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        var packetId = (AppMessageId)Reader.ReadByte();
        msg = IAppMessage.Require(packetId);
        
        var seq = Reader.ReadInt32();
        var isAck = Reader.ReadBoolean();
        if (isAck)
            msg = IAppMessage.RequireAck(packetId);
        
        msg.Sequence = seq;

        bytesNotRead -= read;
        return true;
    }
}