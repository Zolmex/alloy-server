using System.ComponentModel;
using System.Threading;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;

namespace Common.Game.Objects;

public class Entity : IIdentifiable {
    public int Id { get; set; }

    public ObjectDesc Desc;
    public WorldPosData Pos;

    public Entity(ObjectDesc desc) {
        Desc = desc;
    }

    public void Move(float newX, float newY) {
        Pos = new WorldPosData(newX, newY);
    }

    public static Entity Resolve(ushort objType) {
        var desc = XmlLibrary.ObjectDescs[objType];

        // if (desc.Class != null)
        //     switch (desc.Class) {
        //         case "ConnectedWall":
        //         case "CaveWall":
        //             return new ConnectedObject(objType);
        //         case "Portal": // Dungeon portals
        //         case "GuildHallPortal":
        //             var dungeonName = desc.DungeonName;
        //             if (dungeonName != null)
        //                 return new Portal(objType);
        //             break;
        //         case "Character":
        //             if (desc.Enemy)
        //                 return new Enemy(objType);
        //             return new CharacterEntity(objType);
        //         case "ClosedVaultChest":
        //             return new ClosedVaultChest(objType);
        //         case "Container":
        //             return new Container(objType, 8, -1, -1);
        //         case "GuildMerchant":
        //             return new GuildMerchant(objType);
        //     }
        //
        // if (desc.Enemy)
        //     return new Enemy(objType);

        return new Entity(desc);
    }
}