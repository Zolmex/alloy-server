using System;
using System.Runtime.InteropServices;

namespace Common.Structs;

[StructLayout(LayoutKind.Explicit)]
public struct StatValue : IEquatable<StatValue> {
    [FieldOffset(0)] public StatValueType Type; // 1 byte
    [FieldOffset(4)] public int IntVal; // overlaps with Float
    [FieldOffset(4)] public float FloatVal; // overlaps with Int
    [FieldOffset(8)] public string StrVal; // reference, separate slot

    public bool HasValue => Type != StatValueType.None;

    public static StatValue FromInt(int v) {
        return new StatValue { Type = StatValueType.Int, IntVal = v };
    }

    public static StatValue FromFloat(float v) {
        return new StatValue { Type = StatValueType.Float, FloatVal = v };
    }

    public static StatValue FromString(string v) {
        return new StatValue { Type = StatValueType.Str, StrVal = v };
    }

    public bool Equals(StatValue other) {
        if (Type != other.Type) return false;

        return Type switch {
            StatValueType.Int => IntVal == other.IntVal,
            StatValueType.Float => FloatVal.Equals(other.FloatVal),
            StatValueType.Str => string.Equals(StrVal, other.StrVal, StringComparison.Ordinal),
            _ => true // both None
        };
    }

    public override bool Equals(object obj) {
        return obj is StatValue other && Equals(other);
    }

    public override int GetHashCode() {
        return Type switch {
            StatValueType.Int => HashCode.Combine(Type, IntVal),
            StatValueType.Float => HashCode.Combine(Type, FloatVal),
            StatValueType.Str => HashCode.Combine(Type, StrVal),
            _ => 0
        };
    }

    public static bool operator ==(StatValue left, StatValue right) {
        return left.Equals(right);
    }

    public static bool operator !=(StatValue left, StatValue right) {
        return !left.Equals(right);
    }
}

public enum StatValueType : byte {
    None,
    Int,
    Float,
    Str
}