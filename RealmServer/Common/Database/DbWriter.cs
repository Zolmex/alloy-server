#region

using System;

#endregion

namespace Common.Database
{
    public class DbWriter
    {
        public byte[] Write(int[] value)
        {
            if (value == null)
                return null;

            var ret = new byte[value.Length * sizeof(int)];
            Buffer.BlockCopy(value, 0, ret, 0, ret.Length);
            return ret;
        }

        public byte[] Write(float[] value)
        {
            if (value == null)
                return null;

            var ret = new byte[value.Length * sizeof(float)];
            Buffer.BlockCopy(value, 0, ret, 0, ret.Length);
            return ret;
        }

        public byte[] Write(ushort[] value)
        {
            if (value == null)
                return null;

            var ret = new byte[value.Length * sizeof(ushort)];
            Buffer.BlockCopy(value, 0, ret, 0, ret.Length);
            return ret;
        }
    }
}