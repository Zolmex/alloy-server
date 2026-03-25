using Common.Network.Messaging;
using Common.Utilities;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Common.Network;

public class SocketSendState : IDisposable
{
    private byte[] _buffer;
    public int LastValidIndex { get; private set; } = 0;
    public bool HasData => LastValidIndex > 0;

    public SocketSendState()
    {
        _buffer = ArrayPool<byte>.Shared.Rent(0x10000); // 64KB
    }

    public void Reset() => LastValidIndex = 0;

    public void WriteMessage(IAppMessage msg)
    {
        int startPos = LastValidIndex;
        int bodyPos = startPos + 10;

        using (var ms = new MemoryStream(_buffer, bodyPos, _buffer.Length - bodyPos))
            using (var writer = new NetworkWriter(ms))
            {
                msg.Write(writer);
                int bodyLen = (int)ms.Position;
                int totalLen = bodyLen + 10;

                BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan(startPos), totalLen);
                _buffer[startPos + 4] = (byte)msg.MessageId;
                BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan(startPos + 5), msg.Sequence);
                _buffer[startPos + 9] = msg.IsAck ? (byte)1 : (byte)0;

                LastValidIndex += totalLen;
            }
    }

    public void PrepareSAEA(SocketAsyncEventArgs args)
    {
        args.SetBuffer(_buffer, 0, LastValidIndex);
    }

    public void Dispose()
    {
        var buf = Interlocked.Exchange(ref _buffer, null);
        if (buf != null) ArrayPool<byte>.Shared.Return(buf);
    }
}