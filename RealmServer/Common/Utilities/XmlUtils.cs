#region

using System;
using System.Globalization;
using System.Xml.Linq;

#endregion

namespace Common.Utilities
{
    public static class XmlUtils
    {
        private static readonly Logger Log = new(typeof(XmlUtils));

        public static T GetValue<T>(this XElement elem, string name, T def = default)
        {
            if (elem.Element(name) == null)
                return def;

            var value = elem.Element(name).Value;
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(value, type);
            if (type == typeof(byte))
                return (T)Convert.ChangeType(byte.Parse(value), type);
            if (type == typeof(int))
                if (value.Contains('x'))
                    return (T)Convert.ChangeType(Convert.ToInt32(value, 16), type);
                else
                    return (T)Convert.ChangeType(int.Parse(value), type);
            if (type == typeof(short))
                return (T)Convert.ChangeType(short.Parse(value), type);
            if (type == typeof(ushort))
                return (T)Convert.ChangeType(Convert.ToUInt16(value, 16), type);
            if (type == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(value, 16), type);
            if (type == typeof(double))
                return (T)Convert.ChangeType(double.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(float))
                return (T)Convert.ChangeType(float.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(value) || bool.Parse(value), type);

            Log.Error($"Type of {type} is not supported by this method, returning default value: {def}...");
            return def;
        }

        public static T GetAttribute<T>(this XElement elem, string name, T def = default)
        {
            if (elem.Attribute(name) == null)
                return def;

            var value = elem.Attribute(name).Value;
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(value, type);
            if (type == typeof(byte))
                return (T)Convert.ChangeType(byte.Parse(value), type);
            if (type == typeof(int))
                return (T)Convert.ChangeType(int.Parse(value), type);
            if (type == typeof(ushort))
            {
                if (int.TryParse(value, out var val))
                    return (T)Convert.ChangeType(Convert.ToUInt16(val), type);
                else
                    return (T)Convert.ChangeType(Convert.ToUInt16(value, 16), type);
            }

            if (type == typeof(uint))
            {
                if (int.TryParse(value, out var val))
                    return (T)Convert.ChangeType(Convert.ToUInt32(val == -1 ? 0 : val), type);
                else
                    return (T)Convert.ChangeType(Convert.ToUInt32(value, 16), type);
            }

            if (type == typeof(double))
                return (T)Convert.ChangeType(double.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(float))
                return (T)Convert.ChangeType(float.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(value) || bool.Parse(value), type);

            Log.Error($"Type of {type} is not supported by this method, returning default value: {def}...");
            return def;
        }

        public static bool HasElement(this XElement e, string name)
        {
            return e.Element(name) != null;
        }

        public static bool HasAttribute(this XElement e, string name)
        {
            return e.Attribute(name) != null;
        }
    }
}