using Common.Network.Messaging;
using Common.Utilities;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace Common.Network;

public class SocketSendState
{
    public const int HEADER_SIZE = 10;
    
    private readonly MemoryStream _stream;

    private readonly NetworkWriter _writer;
    private TimedLock _lock;

    public byte[] Buffer { get; } = new byte[0x10000]; // 64 KB
    public int LastValidIndex { get; set; } // End position of the packet that still fits within buffer size
    public int Offset { get; set; }

    public SocketSendState()
    {
        _stream = new MemoryStream(Buffer);
        _writer = new NetworkWriter(_stream);
    }

    public void SetBuffer(SocketAsyncEventArgs args)
    {
        Logger.Debug($"Sending {LastValidIndex - Offset} Offset - {Offset}");
        args.SetBuffer(Offset, LastValidIndex - Offset);
    }

    private int PacketBegin() // Returns the start of the packet bytes in the buffer. NEVER use this without locking the SocketSendState instance
    {
        var begin = (int)_stream.Position;
        _stream.Position += HEADER_SIZE; // Leave HEADER_SIZE bytes for the header
        return begin;
    }

    private void PacketEnd(int begin, IAppMessage msg)
    {
        var length = (int)_stream.Position - begin;
        _stream.Position -= length; // Write the header [length][id][sequence][isAck]
        _writer.Write(length);
        _writer.Write((byte)msg.MessageId);
        _writer.Write(msg.Sequence);
        _writer.Write(msg.IsAck);
        _stream.Position += length - HEADER_SIZE; // Go to the next position after packet body to write the next packet

        if (_stream.Position - Offset < Buffer.Length)
            LastValidIndex = (int)_stream.Position;

        // Logger.Debug($"WRITING {pkt} DATA TO BUFFER");
    }

    public void Reset()
    {
        using (TimedLock.Lock(this))
        {
            LastValidIndex = 0;
            Offset = 0;
            _stream.Position = 0;
        }
    }

    public void WriteMessage(IAppMessage msg)
    {
        using (TimedLock.Lock(this))
        {
            var begin = PacketBegin();

            msg.Write(_writer);

            PacketEnd(begin, msg);
        }
    }

    public void BeginSend(SocketAsyncEventArgs args)
    {
        SetBuffer(args);
        Lock();
    }

    public void EndSend()
    {
        // Entire buffer has been written, clear it
        Reset();
        Unlock();
    }

    public void Lock()
    {
        _lock = TimedLock.Lock(this);
    }

    public void Unlock()
    {
        _lock.Dispose();
    }
}