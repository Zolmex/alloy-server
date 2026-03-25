using Common.Network.Messaging;
using Common.Utilities;
using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Common.Network;

public class SocketReceiveState : IDisposable
{
    private byte[] _buffer;
    private int _writePos = 0;
    private int _readPos = 0;
    private const int BUFFER_SIZE = 0x20000;

    public SocketReceiveState()
    {
        // Rent memory from the shared pool
        _buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
    }

    public void PrepareSAEA(SocketAsyncEventArgs args)
    {
        if (_buffer.Length - _writePos < 4096) Compact();
        args.SetBuffer(_buffer, _writePos, _buffer.Length - _writePos);
    }

    public void OnDataReceived(int count) => _writePos += count;

    public bool TryReadMessage(out IAppMessage msg)
    {
        msg = null;
        int available = _writePos - _readPos;
        if (available < 4) return false;

        int length = BitConverter.ToInt32(_buffer, _readPos);
        
        if (length < 10 || length > BUFFER_SIZE)
            throw new InvalidDataException($"Invalid packet length: {length}");

        if (available < length) return false;

        _readPos += 4;
        var packetId = (AppMessageId)_buffer[_readPos++];
        var seq = BitConverter.ToInt32(_buffer, _readPos); _readPos += 4;
        bool isAck = _buffer[_readPos++] != 0;

        msg = isAck ? IAppMessage.RequireAck(packetId) : IAppMessage.Require(packetId);
        msg.Sequence = seq;

        using (var ms = new MemoryStream(_buffer, _readPos, length - 10))
        using (var reader = new NetworkReader(ms))
        {
            msg.Read(reader);
        }

        _readPos += (length - 10);
        return true;
    }

    private void Compact()
    {
        int remaining = _writePos - _readPos;
        if (remaining > 0)
            Buffer.BlockCopy(_buffer, _readPos, _buffer, 0, remaining);
        _writePos = remaining;
        _readPos = 0;
    }

    public void Dispose()
    {
        // Return the buffer to the pool for other connections to use
        var buf = Interlocked.Exchange(ref _buffer, null);
        if (buf != null) ArrayPool<byte>.Shared.Return(buf);
    }
}