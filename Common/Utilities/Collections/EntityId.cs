using System;
using Common.Network;

namespace Common.Utilities.Collections;

public struct EntityId : IEquatable<EntityId> {
    
    public static readonly EntityId Null = new(0, 0);
    
    public readonly int Value;
    public int Index => Value & 0xFFFFF;
    public int Generation => (Value >> 20) & 0xFFF;

    public EntityId(int value) {
        Value = value;
    }
    
    public EntityId(int index, int generation) {
        Value = (generation << 20) | index;
    }

    public static EntityId Read(ref SpanReader rdr)
        => new (rdr.ReadInt32());
    
    public bool Equals(EntityId other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);
    public override int GetHashCode() => Value;

    public static bool operator ==(EntityId a, EntityId b) => a.Value == b.Value;
    public static bool operator !=(EntityId a, EntityId b) => a.Value != b.Value;
}