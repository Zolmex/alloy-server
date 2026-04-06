#region

using Common.Network;
using Common.Utilities;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

#endregion

namespace Common;

public struct IntPoint
{
    public int X;
    public int Y;

    public IntPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(IntPoint a, IntPoint b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(IntPoint a, IntPoint b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public bool Equals(IntPoint other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        if (obj is IntPoint p)
        {
            return Equals(p);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (Y << 16) ^ X;
    }

    public override string ToString()
    {
        return $"X:{X}, Y:{Y}";
    }
}

public struct ObjectStatusData
{
    public int ObjectId;
    public WorldPosData Pos;
    public StatValue[] Stats;
    public StatData[] UpdatedStats;
    public bool Update;

    public static ObjectStatusData Read(ref SpanReader rdr)
    {
        var ret = new ObjectStatusData();
        ret.ObjectId = rdr.ReadInt32();
        ret.Pos = WorldPosData.Read(ref rdr);
        ret.UpdatedStats = new StatData[rdr.ReadByte()];
        for (var i = 0; i < ret.UpdatedStats.Length; i++)
            ret.UpdatedStats[i] = StatData.Read(ref rdr);

        return ret;
    }

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(ObjectId);
        wtr.Write(Pos);
        
        var statCount = 0;
        var pos = wtr.Position;
        wtr.Write((byte)0); // Placeholder
        
        for (var i = 0; i < (int)StatType.StatTypeCount; i++)
        {
            var value = Stats[i];
            if (value.HasValue)
            {
                statCount++;
                StatData.Write(ref wtr, (StatType)i, value);
            }
        }

        var endPos = wtr.Position;
        wtr.Position = pos; // Write in the placeholder the real amount of stat counts
        wtr.Write((byte)statCount);
        wtr.Position = endPos;

        Update = false;
    }

    public void SetStat(StatType type, StatValue value)
    {
        Update = true;
        if (type == StatType.None)
            return;

        var id = (int)type;
        Stats[id] = value;
    }

    public void SetPos(WorldPosData pos)
    {
        Pos = pos;
    }
}

public struct WorldPosData : IEquatable<WorldPosData>
{
    public float X;
    public float Y;

    public WorldPosData(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static WorldPosData Read(ref SpanReader rdr)
    {
        return new WorldPosData { X = rdr.ReadSingle(), Y = rdr.ReadSingle() };
    }


    public override int GetHashCode()
    {
        return (X, Y).GetHashCode();
    }

    public override bool Equals(object other)
    {
        return other is WorldPosData pos && Equals(pos);
    }

    public bool Equals(WorldPosData pos)
    {
        return pos.X == X &&
               pos.Y == Y;
    }

    public static bool operator ==(WorldPosData pos1, WorldPosData pos2)
    {
        return pos1.Equals(pos2);
    }

    public static bool operator !=(WorldPosData pos1, WorldPosData pos2)
    {
        return !pos1.Equals(pos2);
    }

    public static implicit operator Vector2(WorldPosData pos)
    {
        return new Vector2(pos.X, pos.Y);
    }
}

public static class WorldPosDataExtensions
{
    public static Vector2 ToVec2(this WorldPosData data)
    {
        return new Vector2(data.X, data.Y);
    }

    public static float DistSqr(this Vector2 vec1, Vector2 vec2)
    {
        var dx = vec1.X - vec2.X;
        var dy = vec1.Y - vec2.Y;
        return (dx * dx) + (dy * dy);
    }

    public static float AngleDegrees(this WorldPosData pos1, WorldPosData pos2)
    {
        return pos1.AngleRadians(pos2) * 180f / (float)Math.PI;
    }

    public static float AngleRadians(this WorldPosData pos1, WorldPosData pos2)
    {
        return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
    }

    extension(NetworkReader rdr)
    {
        public WorldPosData ReadWorldPosData()
        {
            return new WorldPosData(rdr.ReadSingle(), rdr.ReadSingle());
        }
    }
}

public struct StatData : IEquatable<StatData>
{
    public StatType Type;
    public StatValue Value;

    public StatData(StatType type, StatValue value)
    {
        Type = type;
        Value = value;
    }

    public static bool IsFloatStat(StatType type)
    {
        return type switch
        {
            _ => false
        };
    }

    public static bool IsStringStat(StatType type)
    {
        switch (type)
        {
            case StatType.Name:
            case StatType.GuildName:
            case StatType.InventoryData0:
            case StatType.InventoryData1:
            case StatType.InventoryData2:
            case StatType.InventoryData3:
            case StatType.InventoryData4:
            case StatType.InventoryData5:
            case StatType.InventoryData6:
            case StatType.InventoryData7:
            case StatType.InventoryData8:
            case StatType.InventoryData9:
            case StatType.InventoryData10:
            case StatType.InventoryData11:
            case StatType.InventoryData12:
            case StatType.InventoryData13:
            case StatType.InventoryData14:
            case StatType.InventoryData15:
            case StatType.InventoryData16:
            case StatType.InventoryData17:
            case StatType.InventoryData18:
            case StatType.InventoryData19:
            case StatType.AbilityDataA:
            case StatType.AbilityDataB:
            case StatType.AbilityDataC:
            case StatType.AbilityDataD:
                return true;
            default:
                return false;
        }
    }

    public static StatData Read(ref SpanReader rdr)
    {
        var ret = new StatData();
        ret.Type = (StatType)rdr.ReadByte();
        if (IsStringStat(ret.Type))
            ret.Value = new StatValue() { Type = StatValueType.Str, StrVal = rdr.ReadUTF() };
        else if (IsFloatStat(ret.Type))
            ret.Value = new StatValue() { Type = StatValueType.Float, FloatVal = rdr.ReadSingle() };
        else
            ret.Value = new StatValue() { Type = StatValueType.Int, IntVal = rdr.ReadInt32() };

        return ret;
    }

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)Type);
        if (IsStringStat(Type))
            wtr.WriteUTF(Value.StrVal);
        else if (IsFloatStat(Type))
            wtr.Write(Value.FloatVal);
        else
            wtr.Write(Value.IntVal);
    }

    public static void Write(ref SpanWriter wtr, StatType type, StatValue value)
    {
        wtr.Write((byte)type);
        if (IsStringStat(type))
            wtr.WriteUTF(value.StrVal);
        else if (IsFloatStat(type))
            wtr.Write(value.FloatVal);
        else
            wtr.Write(value.IntVal);
    }
    
    public bool Equals(StatData other)
    {
        if (Type != other.Type) return false;

        return Value.Equals(other.Value);
    }

    public override bool Equals(object obj) => obj is StatData other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Type, Value);
}

public struct TileData
{
    public short X;
    public short Y;
    public ushort GroundType;

    public static TileData Read(NetworkReader rdr)
    {
        return new TileData { X = rdr.ReadInt16(), Y = rdr.ReadInt16(), GroundType = rdr.ReadUInt16() };
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(X);
        wtr.Write(Y);
        wtr.Write(GroundType);
    }
}

public struct ObjectData
{
    public ushort ObjectType;
    public ObjectStatusData Status;

    public static ObjectData Read(ref SpanReader rdr)
    {
        return new ObjectData { ObjectType = rdr.ReadUInt16(), Status = ObjectStatusData.Read(ref rdr) };
    }

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(ObjectType);
        Status.Write(ref wtr);
    }
}

public struct ObjectDropData
{
    public int ObjectId;
    public bool Explode;

    public static ObjectDropData Read(NetworkReader rdr)
    {
        return new ObjectDropData { ObjectId = rdr.ReadInt32(), Explode = rdr.ReadBoolean() };
    }

    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(ObjectId);
        wtr.Write(Explode);
    }
}

public struct SlotObjectData
{
    public int ObjectId;
    public byte SlotId;
}

[StructLayout(LayoutKind.Explicit)]
public struct StatValue : IEquatable<StatValue>
{
    [FieldOffset(0)] public StatValueType Type;    // 1 byte
    [FieldOffset(4)] public int   IntVal;          // overlaps with Float
    [FieldOffset(4)] public float FloatVal;        // overlaps with Int
    [FieldOffset(8)] public string StrVal;         // reference, separate slot

    public bool HasValue => Type != StatValueType.None;

    public static StatValue FromInt(int v)
    {
        return new StatValue { Type = StatValueType.Int, IntVal = v };
    }

    public static StatValue FromFloat(float v)
    {
        return new StatValue { Type = StatValueType.Float, FloatVal = v };
    }

    public static StatValue FromString(string v)
    {
        return new StatValue { Type = StatValueType.Str,  StrVal   = v };
    }
    
    public bool Equals(StatValue other)
    {
        if (Type != other.Type) return false;

        return Type switch
        {
            StatValueType.Int   => IntVal == other.IntVal,
            StatValueType.Float => FloatVal.Equals(other.FloatVal),
            StatValueType.Str   => string.Equals(StrVal, other.StrVal, StringComparison.Ordinal),
            _                   => true // both None
        };
    }

    public override bool Equals(object obj) => obj is StatValue other && Equals(other);

    public override int GetHashCode() => Type switch
    {
        StatValueType.Int   => HashCode.Combine(Type, IntVal),
        StatValueType.Float => HashCode.Combine(Type, FloatVal),
        StatValueType.Str   => HashCode.Combine(Type, StrVal),
        _                   => 0
    };

    public static bool operator ==(StatValue left, StatValue right) => left.Equals(right);
    public static bool operator !=(StatValue left, StatValue right) => !left.Equals(right);
}

public enum StatValueType : byte
{
    None, Int, Float, Str
}

public static class SlotObjectDataExtensions
{
    extension(ref SpanReader rdr)
    {
        public SlotObjectData ReadSlotObjectData()
        {
            return new SlotObjectData { ObjectId = rdr.ReadInt32(), SlotId = rdr.ReadByte() };
        }
    }

    extension(NetworkWriter wtr)
    {
        public void Write(SlotObjectData data)
        {
            wtr.Write(data.ObjectId);
            wtr.Write(data.SlotId);
        }
    }
}