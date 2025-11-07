#region

using Common.Utilities;
using Common.Utilities.Net;
using System;
using System.IO;
using System.Numerics;

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
    public object[] Stats;
    public StatData[] UpdatedStats;
    public bool Update;

    public static ObjectStatusData Read(NetworkReader rdr)
    {
        var ret = new ObjectStatusData();
        ret.ObjectId = rdr.ReadInt32();
        ret.Pos = WorldPosData.Read(rdr);
        ret.UpdatedStats = new StatData[rdr.ReadByte()];
        for (var i = 0; i < ret.UpdatedStats.Length; i++)
            ret.UpdatedStats[i] = StatData.Read(rdr);

        return ret;
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(ObjectId);
        wtr.Write(Pos);
        var statCount = 0;
        var pos = wtr.BaseStream.Position;
        wtr.Write((byte)0); // Placeholder
        using (TimedLock.Lock(Stats))
        {
            for (var i = 0; i < Stats.Length; i++)
            {
                var value = Stats[i];
                if (value != null)
                {
                    statCount++;
                    StatData.Write(wtr, (StatType)i, value);
                }
            }
        }

        var endPos = wtr.BaseStream.Position;
        wtr.BaseStream.Seek(pos, SeekOrigin.Begin); // Write in the placeholder the real amount of stat counts
        wtr.Write((byte)statCount);
        wtr.BaseStream.Seek(endPos, SeekOrigin.Begin);

        Update = false;
    }

    public void SetStat(StatType type, object value)
    {
        Update = true;
        if (type == StatType.None)
            return;

        var id = (int)type;
        using (TimedLock.Lock(Stats))
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

    public static WorldPosData Read(NetworkReader rdr)
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

    public static implicit operator Vector2(WorldPosData pos) => new(pos.X, pos.Y);
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
        return AngleRadians(pos1, pos2) * 180f / (float)Math.PI;
    }

    public static float AngleRadians(this WorldPosData pos1, WorldPosData pos2)
    {
        return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
    }
}

public struct StatData
{
    public StatType Type;
    public object Value;
    public int IntValue;
    public float FloatValue;
    public string TextValue;

    public StatData(StatType type, object value)
    {
        Type = type;
        SetValue(value);
    }

    public void SetValue(object value)
    {
        Value = value;
        if (IsStringStat(Type))
            TextValue = (string)value;
        else if (IsFloatStat(Type))
            FloatValue = (float)value;
        else
            IntValue = (int)value;
    }

    public static bool IsFloatStat(StatType type)
    {
        return type switch
        {
            StatType.CriticalChance => true,
            StatType.DodgeChance => true,
            StatType.CriticalChanceBonus => true,
            StatType.DodgeChanceBonus => true,
            StatType.AttackSpeed => true,
            StatType.AttackSpeedBonus => true,
            StatType.MovementSpeed => true,
            StatType.MovementSpeedBonus => true,
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

    public static StatData Read(NetworkReader rdr)
    {
        var ret = new StatData();
        ret.Type = (StatType)rdr.ReadByte();
        if (IsStringStat(ret.Type))
        {
            ret.TextValue = rdr.ReadUTF();
            ret.Value = ret.TextValue;
        }
        else if (IsFloatStat(ret.Type))
        {
            ret.FloatValue = rdr.ReadSingle();
            ret.Value = ret.FloatValue;
        }
        else
        {
            ret.IntValue = rdr.ReadInt32();
            ret.Value = ret.IntValue;
        }

        return ret;
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Type);
        if (IsStringStat(Type))
            wtr.WriteUTF(TextValue);
        else if (IsFloatStat(Type))
            wtr.Write(FloatValue);
        else
            wtr.Write(IntValue);
    }

    public static void Write(NetworkWriter wtr, StatType type, object value)
    {
        wtr.Write((byte)type);
        if (IsStringStat(type))
            wtr.WriteUTF((string)value);
        else if (IsFloatStat(type))
        {
            var floatValue = value is int intValue ? intValue : (float)value;
            wtr.Write(floatValue);
        }
        else
            wtr.Write(Convert.ToInt32(value));
    }
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

    public static ObjectData Read(NetworkReader rdr)
    {
        return new ObjectData { ObjectType = rdr.ReadUInt16(), Status = ObjectStatusData.Read(rdr) };
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(ObjectType);
        Status.Write(wtr);
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

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(ObjectId);
        wtr.Write(Explode);
    }
}

public struct SlotObjectData
{
    public int ObjectId;
    public byte SlotId;

    public static SlotObjectData Read(NetworkReader rdr)
    {
        return new SlotObjectData { ObjectId = rdr.ReadInt32(), SlotId = rdr.ReadByte() };
    }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(ObjectId);
        wtr.Write(SlotId);
    }
}