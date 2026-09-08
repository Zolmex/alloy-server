using System;
using Arch.Core;
using Common.Network;

namespace Common.Utilities.Collections;

public readonly struct EntityId : IEquatable<EntityId> {
    
    public static readonly EntityId Null = new(Entity.Null);
    
    public readonly long Value;
    public int Index => (int)(Value & 0xFFFFFFFFL); // low 32 bits -> Arch Entity.Id
    public int Version => (int)((Value >> 32) & 0xFFFFFFFFL); // high 32 bits -> entity version

    public EntityId(long value) {
        Value = value;
    }
    
    public EntityId(Entity en) {
        Value = ((long)(uint)en.Version << 32) | (uint)en.Id;
    }

    public static EntityId Read(ref SpanReader rdr)
        => new (rdr.ReadInt64());
    
    public bool Equals(EntityId other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(EntityId a, EntityId b) => a.Value == b.Value;
    public static bool operator !=(EntityId a, EntityId b) => a.Value != b.Value;
    public static bool operator ==(EntityId a, Entity b) => a.Value == ((EntityId)b).Value;
    public static bool operator !=(EntityId a, Entity b) => a.Value != ((EntityId)b).Value;
    
    public static explicit operator EntityId(Entity entity) => new(entity);
}