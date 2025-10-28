using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Resources.Xml.Descriptors;

public abstract class ItemData
{
    public abstract Type FieldsEnum { get; }

    private readonly ConcurrentDictionary<byte, object> _fields = new();
    private readonly Dictionary<byte, Type> _fieldTypes = new();
    protected readonly HashSet<byte> _fieldUpdates = new();
    protected bool _initialized;

    public ItemData Parent;
    public byte ParentField;

    public void ForceFieldUpdate(byte key)
    {
        if (!_fieldTypes.ContainsKey(key))
            throw new Exception($"Field must be initialized before updating its value. key: {key}");

        _fieldUpdates.Add(key);
    }

    public T GetValue<T>(byte key)
    {
        if (!_fields.TryGetValue(key, out var ret))
            return default;
        return (T)ret;
    }

    public void SetValue<T>(byte key, T value, bool isUpdate = false)
    {
        _fields[key] = value;
        if (!isUpdate) // Prevent infinite recursion
            HandleFieldUpdate(key);

        if (_initialized)
        {
            _fieldUpdates.Add(key);
            UpdateParent();
        }
        else
            _fieldTypes[key] = typeof(T); // Set the field type while initializing
    }

    // Only call these methods after initializing the ItemData object.
    // This is used for situations where you're expecting different object types.
    public object GetValue(byte key)
    {
        if (!_initialized)
            return default;

        if (!_fields.TryGetValue(key, out var ret))
            return default;
        return ret;
    }

    protected virtual void HandleFieldUpdate(byte field) { }

    public void SetParent(ItemData parent,
        byte parentField) // Parent field is the field where the parent stores this itemdata object
    {
        Parent = parent;
        ParentField = parentField;
    }

    public void UpdateParent()
    {
        if (Parent == null)
            return;

        Parent.ForceFieldUpdate(ParentField);
        Parent.UpdateParent(); // Update parent's parent
    }

    public void UpdateField(byte field, string strValue)
    {
        if (!_fields.TryGetValue(field, out _))
            return;

        var dataType = _fieldTypes[field];
        var args = strValue.Split("::");
        if (dataType == typeof(int[]))
        {
            var count = int.Parse(args[0]);
            var newVal = new int[count];
            for (var i = 0; i < count; i++)
                newVal[i] = int.Parse(args[i + 1]);

            SetValue(field, newVal);
        }
        else if (dataType == typeof(KeyValuePair<int, int>[])) // Would look something like this: 1::5::5
        {
            var count = int.Parse(args[0]);
            var newVal = new KeyValuePair<int, int>[count];
            for (var i = 0; i < count; i++)
                newVal[i] = new KeyValuePair<int, int>(int.Parse(args[i + 1]), int.Parse(args[i + 2]));

            SetValue(field, newVal);
        }
        else if (dataType == typeof(string))
            SetValue(field, strValue);
        else if (dataType == typeof(int))
            SetValue(field, int.Parse(strValue));
        else if (dataType == typeof(uint))
            SetValue(field, uint.Parse(strValue));
        else if (dataType == typeof(short))
            SetValue(field, short.Parse(strValue));
        else if (dataType == typeof(ushort))
            SetValue(field, ushort.Parse(strValue));
        else if (dataType == typeof(float))
            SetValue(field, float.Parse(strValue));
        else if (dataType == typeof(double))
            SetValue(field, double.Parse(strValue));
        else if (dataType == typeof(bool))
            SetValue(field, bool.Parse(strValue));
        else if (dataType == typeof(byte))
            SetValue(field, byte.Parse(strValue));
        else if (dataType.IsEnum)
            SetValue(field, Enum.Parse(dataType, strValue));
        else if (dataType.IsSubclassOf(typeof(ItemData)))
            return;
    }

    public void Import(BinaryReader reader)
    {
        var fieldCount = reader.ReadByte();
        for (var i = 0; i < fieldCount; i++)
            DeserializeField(reader);
    }

    public virtual List<byte> Export(List<byte> data = null)
    {
        data ??= new List<byte>();
        var updateCount = (byte)_fieldUpdates.Count;
        data.Add(updateCount);
        foreach (var field in _fieldUpdates)
            data = SerializeField(field, data);
        return data;
    }

    public string ExportString(string str = null)
    {
        var fieldCount = _fieldUpdates.Count.ToString();
        str = str == null ? fieldCount : str + "," + fieldCount;
        foreach (var field in _fieldUpdates)
            str = SerializeFieldString(field, str);
        return str;
    }

    private void DeserializeField(BinaryReader reader)
    {
        var field = reader.ReadByte();
        if (!_fields.TryGetValue(field, out _))
            return;

        var dataType = _fieldTypes[field];
        if (dataType == typeof(byte[]))
            SetValue(field, reader.ReadBytes(reader.ReadUInt16()));
        else if (dataType == typeof(int[]))
        {
            var count = reader.ReadUInt16();
            var newVal = new int[count];
            for (var i = 0; i < count; i++)
                newVal[i] = reader.ReadInt32();

            SetValue(field, newVal);
        }
        else if (dataType == typeof(KeyValuePair<int, int>[]))
        {
            var count = reader.ReadUInt16();
            var newVal = new KeyValuePair<int, int>[count];
            for (var i = 0; i < count; i++)
                newVal[i] = new KeyValuePair<int, int>(reader.ReadInt32(), reader.ReadInt32());

            SetValue(field, newVal);
        }
        else if (GetValue(field) is ItemData[] arr)
        {
            var count = reader.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                var data = arr[i];
                data.Import(reader);
                data.SetParent(this, field);
            }
        }
        else if (dataType == typeof(string))
            SetValue(field, reader.ReadUTF());
        else if (dataType == typeof(int))
            SetValue(field, reader.ReadInt32());
        else if (dataType == typeof(uint))
            SetValue(field, reader.ReadUInt32());
        else if (dataType == typeof(short))
            SetValue(field, reader.ReadInt16());
        else if (dataType == typeof(ushort))
            SetValue(field, reader.ReadUInt16());
        else if (dataType == typeof(float))
            SetValue(field, reader.ReadSingle());
        else if (dataType == typeof(double))
            SetValue(field, reader.ReadDouble());
        else if (dataType == typeof(bool))
            SetValue(field, reader.ReadBoolean());
        else if (dataType == typeof(byte))
            SetValue(field, reader.ReadByte());
        else if (dataType.IsEnum)
            SetValue(field, Enum.ToObject(dataType, reader.ReadByte()));
        else if (dataType.IsSubclassOf(typeof(ItemData)))
        {
            var data = (ItemData)GetValue(field);
            data ??= (ItemData)Activator.CreateInstance(dataType,
                null); // Create a new instance if value doesn't exist
            data.Import(reader);
            data.SetParent(this, field);

            SetValue(field, data);
        }
    }

    private unsafe List<byte> SerializeField(byte field, List<byte> data)
    {
        if (!_fields.TryGetValue(field, out var val))
            return data;

        data.Add(field);

        var dataType = val.GetType();
        if (dataType == typeof(byte[]))
        {
            var arr = (byte[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            data.Add(pointer[0]);
            data.Add(pointer[1]);
            for (var i = 0; i < len; i++)
                data.Add(arr[i]);
        }
        else if (dataType == typeof(int[]))
        {
            var arr = (int[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            data.Add(pointer[0]);
            data.Add(pointer[1]);
            for (var i = 0; i < arr.Length; i++)
            {
                var arrVal = arr[i];
                var valPointer = (byte*)&arrVal;
                data.Add(valPointer[0]);
                data.Add(valPointer[1]);
                data.Add(valPointer[2]);
                data.Add(valPointer[3]);
            }
        }
        else if (dataType == typeof(KeyValuePair<int, int>[]))
        {
            var arr = (KeyValuePair<int, int>[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            data.Add(pointer[0]);
            data.Add(pointer[1]);
            for (var i = 0; i < arr.Length; i++)
            {
                var arrKey = arr[i].Key;
                var keyPointer = (byte*)&arrKey;
                data.Add(keyPointer[0]);
                data.Add(keyPointer[1]);
                data.Add(keyPointer[2]);
                data.Add(keyPointer[3]);
                var arrVal = arr[i].Value;
                var valPointer = (byte*)&arrVal;
                data.Add(valPointer[0]);
                data.Add(valPointer[1]);
                data.Add(valPointer[2]);
                data.Add(valPointer[3]);
            }
        }
        else if (val is ItemData[] arr)
        {
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            data.Add(pointer[0]);
            data.Add(pointer[1]);
            foreach (var itemData in arr)
                data = itemData.Export(data);
        }
        else if (dataType == typeof(string))
        {
            var valStr = (string)val;
            var valLen = (short)valStr.Length;
            var valLenPointer = (byte*)&valLen;
            var valStrBytes = Encoding.UTF8.GetBytes(valStr);
            data.Add(valLenPointer[0]);
            data.Add(valLenPointer[1]);
            for (var i = 0; i < valStrBytes.Length; i++)
                data.Add(valStrBytes[i]);
        }
        else if (dataType == typeof(int))
        {
            var valInt = (int)val;
            var valPointer = (byte*)&valInt;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
            data.Add(valPointer[2]);
            data.Add(valPointer[3]);
        }
        else if (dataType == typeof(uint))
        {
            var valUInt = (uint)val;
            var valPointer = (byte*)&valUInt;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
            data.Add(valPointer[2]);
            data.Add(valPointer[3]);
        }
        else if (dataType == typeof(short))
        {
            var valShort = (short)val;
            var valPointer = (byte*)&valShort;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
        }
        else if (dataType == typeof(ushort))
        {
            var valUShort = (ushort)val;
            var valPointer = (byte*)&valUShort;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
        }
        else if (dataType == typeof(float))
        {
            var valFloat = (float)val;
            var valPointer = (byte*)&valFloat;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
            data.Add(valPointer[2]);
            data.Add(valPointer[3]);
        }
        else if (dataType == typeof(double))
        {
            var valDouble = (double)val;
            var valPointer = (byte*)&valDouble;
            data.Add(valPointer[0]);
            data.Add(valPointer[1]);
            data.Add(valPointer[2]);
            data.Add(valPointer[3]);
            data.Add(valPointer[4]);
            data.Add(valPointer[5]);
            data.Add(valPointer[6]);
            data.Add(valPointer[7]);
        }
        else if (dataType == typeof(bool))
        {
            var valBool = (bool)val;
            var valPointer = (byte*)&valBool;
            data.Add(valPointer[0]);
        }
        else if (dataType == typeof(byte) || dataType.IsEnum)
            data.Add((byte)val);
        else if (dataType.IsSubclassOf(typeof(ItemData)))
            data = ((ItemData)val).Export(data);

        return data;
    }

    private unsafe string SerializeFieldString(byte field, string str)
    {
        if (!_fields.TryGetValue(field, out var val))
            return str;

        str += "," + field;

        var dataType = val.GetType();
        if (dataType == typeof(byte[]))
        {
            var arr = (byte[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            str += "," + pointer[0] + "," + pointer[1];
            for (var i = 0; i < len; i++)
                str += "," + arr[i];
        }
        else if (dataType == typeof(int[]))
        {
            var arr = (int[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            str += "," + pointer[0] + "," + pointer[1];
            for (var i = 0; i < arr.Length; i++)
            {
                var arrVal = arr[i];
                var valPointer = (byte*)&arrVal;
                str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3];
            }
        }
        else if (dataType == typeof(KeyValuePair<int, int>[]))
        {
            var arr = (KeyValuePair<int, int>[])val;
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            str += "," + pointer[0] + "," + pointer[1];
            for (var i = 0; i < arr.Length; i++)
            {
                var arrKey = arr[i].Key;
                var keyPointer = (byte*)&arrKey;
                str += "," + keyPointer[0] + "," + keyPointer[1] + "," + keyPointer[2] + "," + keyPointer[3];
                var arrVal = arr[i].Value;
                var valPointer = (byte*)&arrVal;
                str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3];
            }
        }
        else if (val is ItemData[] arr)
        {
            var len = (ushort)arr.Length;
            var pointer = (byte*)&len;
            str += "," + pointer[0] + "," + pointer[1];
            foreach (var itemData in arr)
                str = itemData.ExportString(str);
        }
        else if (dataType == typeof(string))
        {
            var valStr = (string)val;
            var valLen = (short)valStr.Length;
            var valLenPointer = (byte*)&valLen;
            var valStrBytes = Encoding.UTF8.GetBytes(valStr);
            str += "," + valLenPointer[0] + "," + valLenPointer[1];
            for (var i = 0; i < valStrBytes.Length; i++)
                str += "," + valStrBytes[i];
        }
        else if (dataType == typeof(int))
        {
            var valInt = (int)val;
            var valPointer = (byte*)&valInt;
            str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3];
        }
        else if (dataType == typeof(uint))
        {
            var valUInt = (uint)val;
            var valPointer = (byte*)&valUInt;
            str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3];
        }
        else if (dataType == typeof(short))
        {
            var valShort = (short)val;
            var valPointer = (byte*)&valShort;
            str += "," + valPointer[0] + "," + valPointer[1];
        }
        else if (dataType == typeof(ushort))
        {
            var valUShort = (ushort)val;
            var valPointer = (byte*)&valUShort;
            str += "," + valPointer[0] + "," + valPointer[1];
        }
        else if (dataType == typeof(float))
        {
            var valFloat = (float)val;
            var valPointer = (byte*)&valFloat;
            str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3];
        }
        else if (dataType == typeof(double))
        {
            var valDouble = (double)val;
            var valPointer = (byte*)&valDouble;
            str += "," + valPointer[0] + "," + valPointer[1] + "," + valPointer[2] + "," + valPointer[3] + "," +
                   valPointer[4] + "," + valPointer[5] + "," + valPointer[6] + "," + valPointer[7];
        }
        else if (dataType == typeof(bool))
        {
            var valBool = (bool)val;
            var valPointer = (byte*)&valBool;
            str += "," + valPointer[0];
        }
        else if (dataType == typeof(byte))
            str += "," + val;
        else if (dataType.IsEnum)
            str += "," + (byte)val;
        else if (dataType.IsSubclassOf(typeof(ItemData)))
            str = ((ItemData)val).ExportString(str);

        return str;
    }
}