using Common.Network.Messaging;
using Common.Utilities;
using System.IO;
using System.Net.Sockets;

namespace Common.Network;

public class SocketSendState
{
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
        args.SetBuffer(Offset, LastValidIndex - Offset);
    }

    private int PacketBegin() // Returns the start of the packet bytes in the buffer. NEVER use this without locking the SocketSendState instance
    {
        var begin = (int)_stream.Position;
        _stream.Position += 5; // Leave 5 bytes for the header
        return begin;
    }

    private void PacketEnd(int begin, AppMessageId pkt)
    {
        var length = (int)_stream.Position - begin;
        _stream.Position -= length; // Write the header [length][id]
        _writer.Write(length);
        _writer.Write((byte)pkt);
        _stream.Position += length - 5; // Go to the next position after packet body to write the next packet

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

            PacketEnd(begin, msg.MessageId);
        }
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