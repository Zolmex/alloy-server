using System;
using System.ComponentModel;
using System.Threading;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities;

public struct Entity : IEntityIdentifiable, IEquatable<Entity>, IDisposable {
    public EntityId Id { get; set; }
    public ushort ObjectType { get; }
    public EntityType Type { get; }
    public readonly ObjectDesc Desc => XmlLibrary.ObjectDescs[ObjectType];
    
    public Entity(ushort objType) {
        ObjectType = objType;
        Type = ResolveType(objType);
    }

    public readonly bool Equals(Entity other) {
        return Id == other.Id;
    }

    public readonly override bool Equals(object obj) {
        return obj is Entity other && Equals(other);
    }

    public readonly override int GetHashCode() {
        return Id.Value;
    }

    public static bool operator ==(Entity left, Entity right) {
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right) {
        return !left.Equals(right);
    }

    public static EntityType ResolveType(ushort objType) {
        var desc = XmlLibrary.ObjectDescs[objType];
        if (desc.Class != null)
            switch (desc.Class) {
                case "ConnectedWall":
                case "CaveWall":
                case "Wall":
                    return EntityType.StaticObject;
                case "Portal":
                case "GuildHallPortal":
                    return EntityType.Portal;
                case "Character":
                    if (desc.Enemy)
                        return EntityType.Enemy;
                    return EntityType.Character;
                case "ClosedVaultChest":
                case "Container":
                    return EntityType.Container;
                case "Merchant":
                case "GuildMerchant":
                    return EntityType.Merchant;
            }

        if (desc.Enemy)
            return EntityType.Enemy;
        
        if (desc.Static)
            return EntityType.StaticObject;

        if (desc.Player)
            return EntityType.Player;

        return EntityType.GameObject;
    }

    public void Dispose() {
        // Ignore
    }
}