using Common.Network.Messaging;
using System;
using System.IO;
using System.Net.Sockets;

namespace Common.Network;

public class SocketReceiveState
{
    public readonly NetworkReader Reader;

    private int _bytesRead;
    public byte[] Buffer { get; } = new byte[0x10000]; // 64 KB

    public SocketReceiveState()
    {
        Reader = new NetworkReader(new MemoryStream(Buffer));
    }
    
    public void Reset()
    {
        _bytesRead = 0;
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

        var start = (int)Reader.BaseStream.Position;
        Reader.BaseStream.Seek(start + _bytesRead, // In case we failed processing last packet, go to the end of the packet
            SeekOrigin.Begin);
        _bytesRead = 0;
        
        start = (int)Reader.BaseStream.Position;

        var length = Reader.ReadInt32(); // We always start reading from the beginning
        // Log.Debug($"Start: {start} Length: {length} BytesNotRead: {bytesNotRead} BytesRead: {_bytesRead}");

        if (length == 0)
            throw new InvalidDataException("Message length cannot be zero.");

        var read = Math.Min(length - _bytesRead, bytesNotRead); // Bytes read in this current iteration
        _bytesRead += read;

        if (_bytesRead < length) // This means we still have bytes to read that we haven't received in this call
        {
            System.Buffer.BlockCopy(Buffer, start, Buffer, 0,
                _bytesRead); // Move this many bytes to the beginning of the buffer
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        var packetId = (AppMessageId)Reader.ReadByte();
        msg = IAppMessage.Require(packetId);

        bytesNotRead -= read;
        return true;
    }
}