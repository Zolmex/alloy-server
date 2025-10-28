using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ObjectDesc
{
    public readonly XElement XML;
    public readonly string ObjectId;
    public readonly ushort ObjectType;

    public readonly string DisplayId;
    public readonly string DisplayName;
    public readonly string Class;
    public readonly string Group;

    public readonly bool Static;
    public readonly bool CaveWall;
    public readonly bool ConnectedWall;
    public readonly bool BlocksSight;

    public readonly bool OccupySquare;
    public readonly bool FullOccupy;
    public readonly bool EnemyOccupySquare;

    public readonly bool ProtectFromGroundDamage;
    public readonly bool ProtectFromSink;

    public readonly bool Player;
    public readonly bool Enemy;

    public readonly bool God;
    public readonly bool Cube;
    public readonly bool Quest;
    public readonly bool Hero;
    public readonly int Level;
    public readonly bool Oryx;
    public readonly float XpMult;
    public readonly int MinLevel;
    public readonly int MaxLevel;

    public readonly int Size;
    public readonly int MinSize;
    public readonly int MaxSize;

    public readonly int MaxHP;
    public readonly int Defense;

    public readonly string DungeonName;
    public readonly bool RealmPortal;
    public readonly bool KeepInSight;

    public readonly float SpawnProb;
    public readonly SpawnDesc Spawn;
    public readonly TextureDesc Texture;
    public readonly TerrainType Terrain;

    public readonly Dictionary<byte, ProjectileProps> Projectiles;

    public ObjectDesc(XElement e, string id, ushort type)
    {
        XML = e;
        ObjectId = id;
        ObjectType = type;

        DisplayId = e.GetValue<string>("DisplayId", ObjectId);
        DisplayName = DisplayId ?? ObjectId;
        Class = e.GetValue<string>("Class");
        Group = e.GetValue<string>("Group");

        Static = e.HasElement("Static");
        CaveWall = Class == "CaveWall";
        ConnectedWall = Class == "ConnectedWall";
        BlocksSight = e.HasElement("BlocksSight");

        OccupySquare = e.HasElement("OccupySquare");
        FullOccupy = e.HasElement("FullOccupy");
        EnemyOccupySquare = e.HasElement("EnemyOccupySquare");

        ProtectFromGroundDamage = e.HasElement("ProtectFromGroundDamage");
        ProtectFromSink = e.HasElement("ProtectFromSink");

        Enemy = e.HasElement("Enemy");
        Player = e.HasElement("Player");

        God = e.HasElement("God");
        Cube = e.HasElement("Cube");
        Quest = e.HasElement("Quest");
        Hero = e.HasElement("Hero");
        Level = e.GetValue<int>("Level", -1);
        Oryx = e.HasElement("Oryx");
        XpMult = e.GetValue<float>("XpMult", 1);
        MinLevel = e.GetValue<int>("MinLevel");
        MaxLevel = e.GetValue<int>("MaxLevel");

        Size = e.GetValue<int>("Size", 100);
        MinSize = e.GetValue<int>("MinSize", Size);
        MaxSize = e.GetValue<int>("MaxSize", Size);

        MaxHP = e.GetValue<int>("MaxHitPoints", 100);
        Defense = e.GetValue<int>("Defense");

        DungeonName = e.GetValue<string>("DungeonName");
        RealmPortal = e.GetValue<bool>("RealmPortal");

        KeepInSight = e.GetValue<bool>("KeepInSight");

        SpawnProb = e.GetValue<float>("SpawnProb", 1);
        Spawn = e.HasElement("Spawn") ? new SpawnDesc(e.Element("Spawn")) : null;
        Texture = e.HasElement("Texture") ? new TextureDesc(e.Element("Texture")) : null;
        var terrainValue = e.GetValue<string>("Terrain");
        Terrain = terrainValue == null ? TerrainType.None : Enum.Parse<TerrainType>(terrainValue);
        Projectiles = !e.HasElement("Projectile")
            ? null
            : e.Elements("Projectile").Select(i => new ProjectileProps(i)).ToDictionary(i => i.BulletId, i => i);
    }
}