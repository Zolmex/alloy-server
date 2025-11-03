#region

using System.IO;
using System.Net;
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
        Write(Encoding.UTF8.GetBytes(str));
        Write((byte)0);
    }

    public void WriteUTF(string str)
    {
        if (str == null)
            Write((short)0);
        else
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            Write((short)bytes.Length);
            Write(bytes);
        }
    }
    public override void Write(string str)
    {
        base.Write(str);
    }

    public void Write32UTF(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        Write(bytes.Length);
        Write(bytes);
    }
}