using System;
using Common.Network;

namespace Common.Structs;

public struct StatData : IEquatable<StatData> {
    public StatType Type;
    public StatValue Value;

    public StatData(StatType type, StatValue value) {
        Type = type;
        Value = value;
    }

    public static bool IsFloatStat(StatType type) {
        return type switch {
            _ => false
        };
    }

    public static bool IsStringStat(StatType type) {
        switch (type) {
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
                return true;
            default:
                return false;
        }
    }

    public static StatData Read(ref SpanReader rdr) {
        var ret = new StatData();
        ret.Type = (StatType)rdr.ReadByte();
        if (IsStringStat(ret.Type))
            ret.Value = new StatValue { Type = StatValueType.Str, StrVal = rdr.ReadUTF() };
        else if (IsFloatStat(ret.Type))
            ret.Value = new StatValue { Type = StatValueType.Float, FloatVal = rdr.ReadSingle() };
        else
            ret.Value = new StatValue { Type = StatValueType.Int, IntVal = rdr.ReadInt32() };

        return ret;
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Type);
        if (IsStringStat(Type))
            wtr.WriteUTF(Value.StrVal);
        else if (IsFloatStat(Type))
            wtr.Write(Value.FloatVal);
        else
            wtr.Write(Value.IntVal);
    }

    public static void Write(ref SpanWriter wtr, StatType type, StatValue value) {
        wtr.Write((byte)type);
        if (IsStringStat(type))
            wtr.WriteUTF(value.StrVal);
        else if (IsFloatStat(type))
            wtr.Write(value.FloatVal);
        else
            wtr.Write(value.IntVal);
    }

    public bool Equals(StatData other) {
        if (Type != other.Type) return false;

        return Value.Equals(other.Value);
    }

    public override bool Equals(object obj) {
        return obj is StatData other && Equals(other);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Type, Value);
    }
}