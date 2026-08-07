using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Common.Network;

public class SocketReceiveState : IDisposable {
    private byte[] _buffer;
    private int _bytesAvailable;
    private int _bytesRead;

    public SocketReceiveState(int bufferSize) {
        // Rent memory from the shared pool
        _buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
    }

    public void Dispose() {
        // Return the buffer to the pool for other connections to use
        var buf = Interlocked.Exchange(ref _buffer, null);
        if (buf != null)
            ArrayPool<byte>.Shared.Return(buf);
    }

    public void Reset() {
        _bytesAvailable = 0;
        _bytesRead = 0;
    }

    public void PrepareSAEA(SocketAsyncEventArgs args) {
        if (_bytesRead > 0) {
            if (_bytesAvailable > 0)
                Buffer.BlockCopy(_buffer, _bytesRead, _buffer, 0, _bytesAvailable);
            _bytesRead = 0;
        }

        args.SetBuffer(_buffer, _bytesAvailable, _buffer.Length - _bytesAvailable);
    }

    public void OnDataReceived(int count) {
        _bytesAvailable += count; // Total bytes pending to read
        // Console.WriteLine($"RECEIVED {count} BYTES");
    }

    public bool PacketReady() {
        if (_bytesAvailable < 4)
            return false;

        var span = _buffer.AsSpan(_bytesRead, _bytesAvailable);
        var rdr = new SpanReader(span);

        var length = rdr.ReadInt32();

        if (length < 5 || length > _buffer.Length)
            throw new InvalidDataException($"Invalid packet length: {length}");

        if (length > _bytesAvailable)
            return false;

        return true;
    }

    public byte ReadPacket(out SpanReader bodyReader) {
        var span = _buffer.AsSpan(_bytesRead, _bytesAvailable);
        var rdr = new SpanReader(span);

        var length = rdr.ReadInt32();
        var packetId = rdr.ReadByte();

        _bytesAvailable -= length;
        _bytesRead += length;
        if (_bytesAvailable == 0)
            _bytesRead = 0;

        bodyReader = new SpanReader(span.Slice(5, length - 5));

        return packetId;
    }
}