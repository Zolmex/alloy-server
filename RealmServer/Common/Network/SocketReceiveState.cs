using Common.Network.Messaging;
using System;
using System.IO;
using System.Net.Sockets;

namespace Common.Network;

public class SocketReceiveState
{
    private readonly NetworkReader _reader;

    private int _bytesRead;
    public byte[] Buffer { get; } = new byte[0x10000]; // 64 KB

    public void Reset()
    {
        _bytesRead = 0;
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
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
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        var start = (int)_reader.BaseStream.Position;

        var length = _reader.ReadInt32(); // We always start reading from the beginning
        // Log.Debug($"Start: {start} Length: {length} BytesNotRead: {bytesNotRead} BytesRead: {_bytesRead}");

        var read = Math.Min(length - _bytesRead, bytesNotRead); // Bytes read in this current iteration
        _bytesRead += read;

        if (_bytesRead < length) // This means we still have bytes to read that we haven't received in this call
        {
            System.Buffer.BlockCopy(Buffer, start, Buffer, 0,
                _bytesRead); // Move this many bytes to the beginning of the buffer
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        var packetId = (AppMessageId)_reader.ReadByte();
        msg = IAppMessage.Require(packetId);

        _reader.BaseStream.Seek(start + _bytesRead,
            SeekOrigin.Begin); // In case we failed processing, go to the end of the packet
        _bytesRead = 0;

        bytesNotRead -= read;
        return true;
    }
}