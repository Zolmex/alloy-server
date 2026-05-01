using System;
using System.ComponentModel;
using System.Threading;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;

namespace Common.Game;

public struct Entity : IIdentifiable, IEquatable<Entity> {
    public int Id { get; set; }
    public ushort ObjectType { get; }
    public readonly ObjectDesc Desc => XmlLibrary.ObjectDescs[ObjectType];
    
    public WorldPosData Pos;

    public Entity(ushort objType) {
        ObjectType = objType;
    }

    public void Move(float newX, float newY) {
        Pos = new WorldPosData(newX, newY);
    }

    public readonly bool Equals(Entity other) {
        return Id == other.Id;
    }

    public readonly override bool Equals(object obj) {
        return obj is Entity other && Equals(other);
    }

    public readonly override int GetHashCode() {
        return Id;
    }

    public static bool operator ==(Entity left, Entity right) {
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right) {
        return !left.Equals(right);
    }
}