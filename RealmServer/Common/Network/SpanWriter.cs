using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Common.Structs;

namespace Common.Network;

public ref struct SpanWriter {
    public int Position { get; set; }

    private readonly bool _littleEndian;
    private readonly Span<byte> _span;

    private Span<byte> _cursor => _span.Slice(Position);

    public SpanWriter(Span<byte> span, bool littleEndian = true) // Little endian is for network, big endian is for maps
    {
        _span = span;
        _littleEndian = littleEndian;
    }

    public void Write(bool value) {
        _span[Position] = (byte)(value ? 1 : 0);
        Position++;
    }

    public void Write(byte value) {
        _span[Position] = value;
        Position++;
    }

    public void Write(short value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteInt16BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteInt16LittleEndian(_cursor, value);
        Position += 2;
    }

    public void Write(int value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteInt32BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteInt32LittleEndian(_cursor, value);
        Position += 4;
    }

    /// <summary>
    ///     Writes array length as ushort and array to stream.
    ///     Use base method to write without length.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    public void Write<T>(T[] value)
        where T : IConvertible {
        if (value is char[] chars) {
            var span = (ReadOnlySpan<char>)chars.AsSpan();
            var length = Encoding.UTF8.GetByteCount(span);
            Write((ushort)length);

            Encoding.UTF8.GetBytes(span, _cursor); // Writes from 'span' to '_cursor'
            Position += length;
            return;
        }

        if (value is string[] strings) {
            Write((ushort)strings.Length);
            foreach (var str in strings)
                WriteUTF(str);
            return;
        }

        Write((ushort)value.Length);

        if (value is byte[] bytes) {
            bytes.CopyTo(_cursor);
            Position += bytes.Length;
            return;
        }

        for (var i = 0; i < value.Length; i++)
            switch (value[i]) {
                case bool @bool: Write(@bool); break;
                case byte @byte: Write(@byte); break; // this one will probably never work because of byte[] check above
                case char @char: Write(@char); break;
                case double @double: Write(@double); break;
                case short @short: Write(@short); break;
                case int @int: Write(@int); break;
                case long @long: Write(@long); break;
                case sbyte @sbyte: Write(@sbyte); break;
                case float @float: Write(@float); break;
                case string @string: WriteUTF(@string); break;
                case ushort @ushort: Write(@ushort); break;
                case uint @uint: Write(@uint); break;
                case ulong @ulong: Write(@ulong); break;
                default: throw new NotSupportedException($"{value[i].GetType()} not supported.");
            }
    }

    public void Write(string[] value) {
        Write((ushort)value.Length);
        for (var i = 0; i < value.Length; i++)
            WriteUTF(value[i]);
    }

    public void Write(long value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteInt64BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteInt64LittleEndian(_cursor, value);
        Position += 8;
    }

    public void Write(ushort value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteUInt16BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteUInt16LittleEndian(_cursor, value);
        Position += 2;
    }

    public void Write(uint value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteUInt32BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteUInt32LittleEndian(_cursor, value);
        Position += 4;
    }

    public void Write(ulong value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteUInt64BigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteUInt64LittleEndian(_cursor, value);
        Position += 8;
    }

    public void Write(float value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteSingleBigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteSingleLittleEndian(_cursor, value);
        Position += 4;
    }

    public void Write(double value) {
        if (!_littleEndian)
            BinaryPrimitives.WriteDoubleBigEndian(_cursor, value);
        else
            BinaryPrimitives.WriteDoubleLittleEndian(_cursor, value);
        Position += 8;
    }

    public void WriteNullTerminatedString(ReadOnlySpan<char> str) {
        var length = Encoding.UTF8.GetByteCount(str);
        Encoding.UTF8.GetBytes(str, _cursor);
        Position += length;

        _span[Position] = 0;
        Position++;
    }

    public void WriteUTF(ReadOnlySpan<char> str) {
        if (str.IsEmpty) {
            Write(ushort.MinValue);
        }
        else {
            var length = Encoding.UTF8.GetByteCount(str);
            if (Position + length + 2 > _span.Length)
                throw new InternalBufferOverflowException(
                    $"SpanWriter buffer is too small. Current: {Position} | Target total: {length + 2} | Max: {_span.Length}");

            Write((ushort)length);

            Encoding.UTF8.GetBytes(str, _cursor);
            Position += length;
        }
    }

    public void Write(WorldPosData wp) {
        Write(wp.X);
        Write(wp.Y);
    }

    public void Write32UTF(string str) {
        var length = Encoding.UTF8.GetByteCount(str);
        Write(length);

        Encoding.UTF8.GetBytes(str, _cursor);
        Position += length;
    }
}