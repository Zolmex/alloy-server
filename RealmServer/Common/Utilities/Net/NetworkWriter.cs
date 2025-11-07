#region

using System.IO;
using System.Net;
using System.Numerics;
using System.Text;

#endregion

namespace Common.Utilities.Net;

public class NetworkWriter : BinaryWriter
{
    private readonly bool _littleEndian;

    public NetworkWriter(Stream s, bool littleEndian = true) : base(s, Encoding.UTF8) // Little endian is for network, big endian is for maps
    {
        _littleEndian = littleEndian;
    }

    public override void Write(short value)
    {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
    }

    public override void Write(int value)
    {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
    }

    /// <summary>
    /// Writes array length as ushort and array to stream.
    /// Use base method to write without length.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    public void Write<T>(T[] value)
    where T : struct, INumber<T>
    {
        Write((ushort)value.Length);

        if (value is byte[] bytes)
        {
            base.Write(bytes);
            return;
        }

        for (var i = 0; i < value.Length; i++)
        {
            switch (value[i])
            {
                case long l: Write(l); break;
                case float f: Write(f); break;
                case double d: Write(d); break;
                case decimal dec: Write(dec); break;
                case short s: Write(s); break;
                case ushort us: Write(us); break;
                case uint ui: Write(ui); break;
                case ulong ul: Write(ul); break;
                case int integer: Write(integer); break;
            }
        }
    }
    public void Write(string[] value)
    {
        Write((ushort)value.Length);
        for (var i = 0; i < value.Length; i++)
        {
            Write(value[i]);
        }
    }
    public override void Write(long value)
    {
        if (!_littleEndian)
            base.Write(IPAddress.HostToNetworkOrder(value));
        else
            base.Write(value);
    }

    public override void Write(ushort value)
    {
        if (!_littleEndian)
            base.Write((ushort)IPAddress.HostToNetworkOrder((short)value));
        else
            base.Write(value);
    }

    public override void Write(uint value)
    {
        if (!_littleEndian)
            base.Write((uint)IPAddress.HostToNetworkOrder((int)value));
        else
            base.Write(value);
    }

    public override void Write(ulong value)
    {
        if (!_littleEndian)
            base.Write((ulong)IPAddress.HostToNetworkOrder((long)value));
        else
            base.Write(value);
    }

    public override unsafe void Write(float value)
    {
        if (!_littleEndian)
            for (var i = 3; i >= 0; i--)
                base.Write(((byte*)&value)[i]);
        else
            base.Write(value);
    }

    public override unsafe void Write(double value)
    {
        if (!_littleEndian)
            for (var i = 7; i >= 0; i--)
                base.Write(((byte*)&value)[i]);
        else
            base.Write(value);
    }

    public void WriteNullTerminatedString(string str)
    {
        base.Write(Encoding.UTF8.GetBytes(str));
        Write((byte)0);
    }

    public void WriteUTF(string str)
    {
        if (str == null)
            Write((ushort)0);
        else
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            Write(bytes);
        }
    }
    public override void Write(string str)
    {
        WriteUTF(str);
    }
    public void Write(WorldPosData wp)
    {
        Write(wp.X);
        Write(wp.Y);
    }
    public void Write32UTF(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        Write(bytes.Length);
        base.Write(bytes);
    }
}