using Common.Network.Messaging;
using Common.Utilities;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Common.Network;

public class SocketSendState : IDisposable
{
    private byte[] _writeBuffer;
    private byte[] _sendBuffer;

    private int _writeLength;
    private int _sendLength;
    private int _sendOffset;
    private bool _pending;
    
    public SocketSendState()
    {
        _sendBuffer = ArrayPool<byte>.Shared.Rent(0x20000);
        Array.Clear(_sendBuffer);
        _writeBuffer = ArrayPool<byte>.Shared.Rent(0x20000);
        Array.Clear(_writeBuffer);
    }

    public void Reset()
    {
        _writeLength = 0;
        _sendLength = 0;
        _sendOffset = 0;
        _pending = false;
    }

    public bool WriteMessage(IAppMessage msg)
    {
        using (TimedLock.Lock(this))
        {
            int startPos = _writeLength;
            var bodyStart = startPos + 10;

            var span = _writeBuffer.AsSpan();
            var writer = new SpanWriter(span);
            writer.Position = bodyStart;
            
            msg.Write(ref writer);

            int totalLen = writer.Position - startPos;

            writer.Position = startPos;
            writer.Write(totalLen);
            writer.Write((byte)msg.MessageId);
            writer.Write(msg.Sequence);
            writer.Write((byte)(msg.IsAck ? 1 : 0));

            _writeLength += totalLen;
        }

        return _writeLength < 48000;
    }
    
    public void WritePacket(IWritable pkt, byte pktId)
    {
        using (TimedLock.Lock(this))
        {
            int start = _writeLength;

            var span = _writeBuffer.AsSpan();

            int bodyStart = start + 5;

            var writer = new SpanWriter(span); // assume you have or can make one
            writer.Position = bodyStart;

            pkt.Write(ref writer);

            int totalLen = writer.Position - start;

            writer.Position = start;
            writer.Write(totalLen);
            writer.Write(pktId);

            _writeLength += totalLen;
        }
    }
    
    public bool TryBeginSend(SocketAsyncEventArgs args) {
        using (TimedLock.Lock(this)) {
            if (_pending)
                return false;

            if (_writeLength == 0)
            {
                _pending = false;
                return false;
            }
            
            _pending = true;
            
            var tmp = _sendBuffer;
            _sendBuffer = _writeBuffer;
            _writeBuffer = tmp;

            _sendLength = _writeLength;
            _writeLength = 0;
            _sendOffset = 0;

            args.SetBuffer(_sendBuffer, 0, _sendLength);
        }

        return true;
    }

    public bool OnDataSent(SocketAsyncEventArgs args) // If all bytes were transferred, lastvalidindex will be 0 again
    {
        using (TimedLock.Lock(this))
        {
            _sendOffset += args.BytesTransferred;

            if (_sendOffset < _sendLength)
            {
                // continue sending remaining bytes
                args.SetBuffer(_sendBuffer, _sendOffset, _sendLength - _sendOffset);
                return true; // continue send
            }

            // done
            _sendLength = 0;
            _sendOffset = 0;

            _pending = false;
            return false;
        }
    }
    
    public void Dispose()
    {
        // Return the buffer to the pool for other connections to use
        var buf = Interlocked.Exchange(ref _sendBuffer, null);
        if (buf != null)
            ArrayPool<byte>.Shared.Return(buf);
        buf = Interlocked.Exchange(ref _writeBuffer, null);
        if (buf != null)
            ArrayPool<byte>.Shared.Return(buf);
    }
}