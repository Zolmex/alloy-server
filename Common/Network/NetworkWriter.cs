#region

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Common.Structs;

#endregion

namespace Common.Network;

public class NetworkWriter : BinaryWriter {
    private readonly bool _littleEndian;

    public NetworkWriter(Stream s, bool littleEndian = true) :
        base(s, Encoding.UTF8) // Little endian is for network, big endian is for maps
    {
        _littleEndian = littleEndian;
    }

    public override void Write(short value) {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
    }

    public override void Write(int value) {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
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
            var span = chars.AsSpan();
            Write((ushort)Encoding.UTF8.GetByteCount(span));
            base.Write(span);
            return;
        }

        if (value is string[] strings) {
            Write(strings);
            return;
        }

        Write((ushort)value.Length);

        if (value is byte[] bytes) {
            base.Write(bytes);
            return;
        }

        for (var i = 0; i < value.Length; i++)
            switch (value[i]) {
                case bool @bool: Write(@bool); break;
                case byte @byte: Write(@byte); break; // this one will probably never work because of byte[] check above
                case char @char: Write(@char); break;
                case decimal @decimal: Write(@decimal); break;
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
            }
    }

    public void Write(string[] value) {
        Write((ushort)value.Length);
        for (var i = 0; i < value.Length; i++) WriteUTF(value[i]);
    }

    public override void Write(long value) {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
    }

    public override void Write(ushort value) {
        if (!_littleEndian)
            base.Write((ushort)IPAddress.HostToNetworkOrder((short)value));
        else
            base.Write(value);
    }

    public override void Write(uint value) {
        if (!_littleEndian)
            base.Write((uint)IPAddress.HostToNetworkOrder((int)value));
        else
            base.Write(value);
    }

    public override void Write(ulong value) {
        if (!_littleEndian)
            base.Write((ulong)IPAddress.HostToNetworkOrder((long)value));
        else
            base.Write(value);
    }

    public override unsafe void Write(float value) {
        if (!_littleEndian)
            for (var i = 3; i >= 0; i--)
                base.Write(((byte*)&value)[i]);
        else
            base.Write(value);
    }

    public override unsafe void Write(double value) {
        if (!_littleEndian)
            for (var i = 7; i >= 0; i--)
                base.Write(((byte*)&value)[i]);
        else
            base.Write(value);
    }

    public void WriteNullTerminatedString(ReadOnlySpan<char> str) {
        base.Write(str);
        base.Write(byte.MinValue);
    }

    private void WriteUTF(ReadOnlySpan<char> str) {
        if (str.IsEmpty) {
            Write(ushort.MinValue);
        }
        else {
            Write((ushort)str.Length);
            base.Write(str);
        }
    }

    public void Write(WorldPosData wp) {
        Write(wp.X);
        Write(wp.Y);
    }

    public void Write32UTF(string str) {
        var bytes = Encoding.UTF8.GetBytes(str);
        Write(bytes.Length);
        base.Write(bytes);
    }
}

public static class NetworkWriterExtension {
    extension(NetworkWriter wtr) {
        public void Write<T>(T value) where T : struct, Enum {
            wtr.Write(MemoryMarshal.AsBytes(new ReadOnlySpan<T>(in value)));
        }

        public void Write<T>(T[] value) where T : struct, Enum {
            wtr.Write((ushort)value.Length);
            wtr.Write(MemoryMarshal.AsBytes(value));
        }
        //public void Write(TradeItem value)
        //{
        //    wtr.Write(value.Item);
        //    wtr.Write(value.SlotType);
        //    wtr.Write(value.Tradeable);
        //    wtr.Write(value.Included);
        //}
    }
}