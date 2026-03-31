using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Common.Network;

public ref struct SpanReader
{
    public int Position { get; set; }
    
    private readonly bool _littleEndian;
    private readonly ReadOnlySpan<byte> _span;
    
    private ReadOnlySpan<byte> _cursor => _span.Slice(Position);

    public SpanReader(ReadOnlySpan<byte> span, bool littleEndian = true) // Little endian is for network, big endian is for maps
    {
        _span = span;
        _littleEndian = littleEndian;
        Position = 0;
    }

    public bool ReadBoolean()
    {
        var ret = _span[Position] != 0;
        Position++;
        return ret;
    }

    public byte ReadByte()
    {
        var ret = _span[Position];
        Position++;
        return ret;
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        var ret = _span.Slice(Position, length);
        Position += length;
        return ret;
    }
    
    public short ReadInt16()
    {
        short ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadInt16BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadInt16LittleEndian(_cursor);
        
        Position += 2;
        return ret;
    }

    public int ReadInt32()
    {
        int ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadInt32BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadInt32LittleEndian(_cursor);
        
        Position += 4;
        return ret;
    }

    public long ReadInt64()
    {
        long ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadInt64BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadInt64LittleEndian(_cursor);
        
        Position += 8;
        return ret;
    }

    public ushort ReadUInt16()
    {
        ushort ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadUInt16BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadUInt16LittleEndian(_cursor);
        
        Position += 2;
        return ret;
    }

    public uint ReadUInt32()
    {
        uint ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadUInt32BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadUInt32LittleEndian(_cursor);
        
        Position += 4;
        return ret;
    }

    public ulong ReadUInt64()
    {
        ulong ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadUInt64BigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadUInt64LittleEndian(_cursor);
        
        Position += 8;
        return ret;
    }

    public float ReadSingle()
    {
        float ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadSingleBigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadSingleLittleEndian(_cursor);
        
        Position += 4;
        return ret;
    }

    public double ReadDouble()
    {
        double ret;
        if (!_littleEndian)
            ret = BinaryPrimitives.ReadDoubleBigEndian(_cursor);
        else
            ret = BinaryPrimitives.ReadDoubleLittleEndian(_cursor);
        
        Position += 8;
        return ret;
    }

    public string ReadNullTerminatedString()
    {
        int relativeNullIndex = _cursor.IndexOf((byte)0); // Index of the next zero-byte relative to the current position
        if (relativeNullIndex == -1)
            throw new InvalidDataException("String not null terminated");

        var bytes = _span.Slice(Position, relativeNullIndex);
        string result = Encoding.UTF8.GetString(bytes);
    
        Position += relativeNullIndex + 1; 
        return result;
    }

    public string ReadUTF()
    {
        ushort length;
        if (!_littleEndian)
            length = BinaryPrimitives.ReadUInt16BigEndian(_cursor);
        else
            length = BinaryPrimitives.ReadUInt16LittleEndian(_cursor);
        Position += 2;

        var ret = Encoding.UTF8.GetString(_span.Slice(Position, length));
        Position += length;
        return ret;
    }

    public string Read32UTF()
    {
        int length;
        if (!_littleEndian)
            length = BinaryPrimitives.ReadInt32BigEndian(_cursor);
        else
            length = BinaryPrimitives.ReadInt32LittleEndian(_cursor);
        Position += 4;
        
        var ret = Encoding.UTF8.GetString(_span.Slice(Position, length));
        Position += length;
        return ret;
    }
}