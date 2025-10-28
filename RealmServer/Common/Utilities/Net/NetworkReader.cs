#region

using System;
using System.IO;
using System.Net;
using System.Text;

#endregion

namespace Common.Utilities.Net
{
    public class NetworkReader : BinaryReader
    {
        private readonly bool _littleEndian;

        public NetworkReader(Stream s, bool littleEndian = true) : base(s, Encoding.UTF8) // Little endian is for network, big endian is for maps
        {
            _littleEndian = littleEndian;
        }

        public override short ReadInt16()
        {
            if (!_littleEndian)
                return IPAddress.NetworkToHostOrder(base.ReadInt16());
            else
                return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (!_littleEndian)
                return IPAddress.NetworkToHostOrder(base.ReadInt32());
            else
                return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (!_littleEndian)
                return IPAddress.NetworkToHostOrder(base.ReadInt64());
            else
                return base.ReadInt64();
        }

        public override ushort ReadUInt16()
        {
            if (!_littleEndian)
                return (ushort)IPAddress.NetworkToHostOrder((short)base.ReadUInt16());
            else
                return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (!_littleEndian)
                return (uint)IPAddress.NetworkToHostOrder((int)base.ReadUInt32());
            else
                return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (!_littleEndian)
                return (ulong)IPAddress.NetworkToHostOrder((long)base.ReadUInt64());
            else
                return base.ReadUInt64();
        }

        public override float ReadSingle()
        {
            if (!_littleEndian)
            {
                var arr = base.ReadBytes(4);
                Array.Reverse(arr);
                return BitConverter.ToSingle(arr, 0);
            }
            else
                return base.ReadSingle();
        }

        public override double ReadDouble()
        {
            if (!_littleEndian)
            {
                var arr = base.ReadBytes(8);
                Array.Reverse(arr);
                return BitConverter.ToDouble(arr, 0);
            }
            else
                return base.ReadDouble();
        }

        public string ReadNullTerminatedString()
        {
            var ret = new StringBuilder();
            var b = ReadByte();
            while (b != 0)
            {
                ret.Append((char)b);
                b = ReadByte();
            }

            return ret.ToString();
        }

        public string ReadUTF()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadInt16()));
        }

        public string Read32UTF()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadInt32()));
        }
    }
}