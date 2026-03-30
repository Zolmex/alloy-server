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
    private int _bytesAvailable;
    private int _bytesRead;
    private const int BUFFER_SIZE = 0x20000;

    private readonly MemoryStream _stream;
    private readonly NetworkReader _reader;
    
    public SocketReceiveState()
    {
        // Rent memory from the shared pool
        _buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
        _stream = new MemoryStream(_buffer);
        _reader = new NetworkReader(_stream);
    }

    public void Reset()
    {
        _bytesAvailable = 0;
        _bytesRead = 0;
    }
    
    public void PrepareSAEA(SocketAsyncEventArgs args)
    {
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

    public bool TryReadMessage(out IAppMessage msg)
    {
        msg = null;
        if (_bytesAvailable < 4)
            return false;

        _stream.Seek(_bytesRead, SeekOrigin.Begin); // Make sure we are at the next packet position
        
        int length = _reader.ReadInt32();
        // Console.WriteLine($"Length {length} bytes");
        if (length < 10 || length > BUFFER_SIZE)
            throw new InvalidDataException($"Invalid packet length: {length}");

        if (length > _bytesAvailable)
            return false;

        var packetId = (AppMessageId)_reader.ReadByte();
        var seq = _reader.ReadInt32();
        bool isAck = _reader.ReadByte() != 0;

        msg = isAck ? IAppMessage.RequireAck(packetId) : IAppMessage.Require(packetId);
        msg.Sequence = seq;

        msg.Read(_reader);

        _bytesAvailable -= length;
        _bytesRead += length;
        if (_bytesAvailable == 0)
            _bytesRead = 0;
        
        return true;
    }
    
    public bool TryReadPacket(out (byte, NetworkReader) ret)
    {
        ret = default;
        if (_bytesAvailable < 4)
            return false;

        _stream.Seek(_bytesRead, SeekOrigin.Begin); // Make sure we are at the next packet position
        
        int length = _reader.ReadInt32();
        
        if (length < 5 || length > BUFFER_SIZE)
            throw new InvalidDataException($"Invalid packet length: {length}");

        if (length > _bytesAvailable)
            return false;

        var packetId = _reader.ReadByte();
        
        ret.Item2 = _reader;

        ret.Item1 = packetId;

        _bytesAvailable -= length;
        _bytesRead += length;
        if (_bytesAvailable == 0)
            _bytesRead = 0;
        
        return true;
    }

    public void Dispose()
    {
        // Return the buffer to the pool for other connections to use
        var buf = Interlocked.Exchange(ref _buffer, null);
        if (buf != null)
            ArrayPool<byte>.Shared.Return(buf);
        _stream.Dispose();
        _reader.Dispose();
    }
}