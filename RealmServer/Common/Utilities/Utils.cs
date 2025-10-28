#region

using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Common.Utilities
{
    public static class Utils
    {
        public static string ToSHA1(this string value)
        {
            return Convert.ToBase64String(new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        public static bool ContainsIgnoreCase(this string self, string val)
        {
            return self.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        public static bool EqualsIgnoreCase(this string self, string val)
        {
            return self.Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }

        public static T GetEnumValue<T>(string val)
        {
            return val == null ? default : (T)Enum.Parse(typeof(T), val.Replace(" ", ""));
        }

        // https://stackoverflow.com/a/6553303
        public static bool IsDefaultValue(Type type, object obj)
        {
            if (obj == null)
                return true;
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
                return false;

            var defaultValue = Activator.CreateInstance(type);
            return obj.Equals(defaultValue);
        }

        public static object FromPrefix<T>(string x)
        {
            if (typeof(T) == typeof(long))
                return x.StartsWith("0x") ? long.Parse(x.Substring(2), NumberStyles.HexNumber) : long.Parse(x);
            if (typeof(T) == typeof(ulong))
                return x.StartsWith("0x") ? ulong.Parse(x.Substring(2), NumberStyles.HexNumber) : ulong.Parse(x);
            if (typeof(T) == typeof(int))
                return x.StartsWith("0x") ? int.Parse(x.Substring(2), NumberStyles.HexNumber) : int.Parse(x);
            if (typeof(T) == typeof(uint))
                return x.StartsWith("0x") ? uint.Parse(x.Substring(2), NumberStyles.HexNumber) : uint.Parse(x);
            if (typeof(T) == typeof(short))
                return x.StartsWith("0x") ? short.Parse(x.Substring(2), NumberStyles.HexNumber) : short.Parse(x);
            if (typeof(T) == typeof(ushort))
                return x.StartsWith("0x") ? ushort.Parse(x.Substring(2), NumberStyles.HexNumber) : ushort.Parse(x);
            if (typeof(T).IsSubclassOf(typeof(Enum)))
                return Enum.Parse(typeof(T), x);
            throw new ArgumentException($"Type {typeof(T)} is not supported.");
        }

        public static string ReadUTF(this BinaryReader reader)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt16()));
        }

        public static string ReadUTF32(this BinaryReader reader)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        }

        public static void WriteUTF(this BinaryWriter writer, string str)
        {
            if (str == null)
                writer.Write((short)0);
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                writer.Write((short)bytes.Length);
                writer.Write(bytes);
            }
        }

        public static void WriteUTF32(this BinaryWriter writer, string str)
        {
            if (str == null)
                writer.Write(0);
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }

        public static int FindCircleCircleIntersections(Vector2 c0, float r0, Vector2 c1, float r1, out Vector2 intersection1, out Vector2 intersection2)
        {
            // Find the distance between the centers.
            double dx = c0.X - c1.X;
            double dy = c0.Y - c1.Y;
            var dist = Math.Sqrt((dx * dx) + (dy * dy));

            if (Math.Abs(dist - (r0 + r1)) < 0.00001)
            {
                intersection1 = Vector2.Lerp(c0, c1, r0 / (r0 + r1));
                intersection2 = intersection1;
                return 1;
            }

            // See how many solutions there are.
            if (dist > r0 + r1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(r0 - r1))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (dist == 0 && r0 == r1)
            {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                var a = ((r0 * r0) -
                    (r1 * r1) + (dist * dist)) / (2 * dist);
                var h = Math.Sqrt((r0 * r0) - (a * a));

                // Find P2.
                var cx2 = c0.X + (a * (c1.X - c0.X) / dist);
                var cy2 = c0.Y + (a * (c1.Y - c0.Y) / dist);

                // Get the points P3.
                intersection1 = new Vector2(
                    (float)(cx2 + (h * (c1.Y - c0.Y) / dist)),
                    (float)(cy2 - (h * (c1.X - c0.X) / dist)));
                intersection2 = new Vector2(
                    (float)(cx2 - (h * (c1.Y - c0.Y) / dist)),
                    (float)(cy2 + (h * (c1.X - c0.X) / dist)));

                return 2;
            }
        }

        public static float Rad2Deg(this float rad)
        {
            return rad * (180f / MathF.PI);
        }

        public static float Deg2Rad(this float deg)
        {
            return deg * (MathF.PI / 180f);
        }

        public static float? Rad2Deg(this float? rad)
        {
            return rad * (180f / MathF.PI);
        }

        public static float? Deg2Rad(this float? deg)
        {
            return deg * (MathF.PI / 180f);
        }

        public static int Rad2Deg(this int rad)
        {
            return (int)(rad * (180f / MathF.PI));
        }

        public static int Deg2Rad(this int deg)
        {
            return (int)(deg * (MathF.PI / 180f));
        }

        // https://www.reddit.com/r/csharp/comments/r2h05h/comment/hm53q2e/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
        public static void SafeResult(this Task t)
        {
            t.GetAwaiter().GetResult();
        }

        public static T SafeResult<T>(this Task<T> t)
        {
            return t.GetAwaiter().GetResult();
        }

        public static T SafeResult<T>(this ValueTask<T> t)
        {
            return t.GetAwaiter().GetResult();
        }
    }
}