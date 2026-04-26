using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class ObjectDesc {
    public readonly bool BlocksSight;
    public readonly bool CaveWall;
    public readonly string Class;
    public readonly bool ConnectedWall;
    public readonly bool Cube;
    public readonly int Defense;

    public readonly string DisplayId;
    public readonly string DisplayName;

    public readonly string DungeonName;
    public readonly bool Enemy;
    public readonly bool EnemyOccupySquare;
    public readonly bool FullOccupy;

    public readonly bool God;
    public readonly string Group;
    public readonly bool Hero;
    public readonly bool KeepInSight;
    public readonly int Level;

    public readonly int MaxHP;
    public readonly int MaxLevel;
    public readonly int MaxSize;
    public readonly int MinLevel;
    public readonly int MinSize;
    public readonly string ObjectId;
    public readonly ushort ObjectType;

    public readonly bool OccupySquare;
    public readonly bool Oryx;

    public readonly bool Player;

    public readonly ProjectileCollection Projectiles;

    public readonly bool ProtectFromGroundDamage;
    public readonly bool ProtectFromSink;
    public readonly bool Quest;
    public readonly bool RealmPortal;

    public readonly int Size;
    public readonly SpawnDesc Spawn;

    public readonly float SpawnProb;

    public readonly bool Static;
    public readonly TerrainType Terrain;
    public readonly TextureDesc Texture;
    public readonly XElement XML;
    public readonly float XpMult;

    public ObjectDesc(XElement e, string id, ushort type) {
        XML = e;
        ObjectId = id;
        ObjectType = type;

        DisplayId = e.GetValue("DisplayId", ObjectId);
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
        Level = e.GetValue("Level", -1);
        Oryx = e.HasElement("Oryx");
        XpMult = e.GetValue<float>("XpMult", 1);
        MinLevel = e.GetValue<int>("MinLevel");
        MaxLevel = e.GetValue<int>("MaxLevel");

        Size = e.GetValue("Size", 100);
        MinSize = e.GetValue("MinSize", Size);
        MaxSize = e.GetValue("MaxSize", Size);

        MaxHP = e.GetValue("MaxHitPoints", 100);
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
            ? new ProjectileCollection([])
            : new ProjectileCollection(e.Elements("Projectile").Select(i => new ContainerProjectileProps(i)));
    }
}

public class ProjectileCollection {
    private readonly ConcurrentDictionary<byte, ContainerProjectileProps> _customDict = new(); // Behavior projectiles

    private readonly Dictionary<byte, ContainerProjectileProps> _dict = new(); // XML projectiles
    private byte _nextProjId;

    public ProjectileCollection(IEnumerable<ContainerProjectileProps> props) {
        foreach (var prop in props) {
            _dict.TryAdd(prop.ProjectileIndex, prop);
            _nextProjId = Math.Max(_nextProjId, prop.ProjectileIndex);
        }

        _nextProjId++;
    }

    public ContainerProjectileProps this[byte projId] {
        get {
            if (_dict.TryGetValue(projId, out var ret) || _customDict.TryGetValue(projId, out ret))
                return ret;

            throw new KeyNotFoundException($"Key: {projId}");
        }
    }

    public ICollection<ContainerProjectileProps> Custom => _customDict.Values;

    public bool TryGetValue(byte projId, out ContainerProjectileProps props) {
        return _dict.TryGetValue(projId, out props) || _customDict.TryGetValue(projId, out props);
    }

    public ContainerProjectileProps AddOrGet(string objectId, int lifetimeMs, float speed, int damage = -1,
        int minDamage = -1,
        int maxDamage = -1, (ConditionEffectIndex, int)[] effects = null, bool multiHit = false,
        bool passesCover = false, bool armorPiercing = false,
        bool wavy = false, bool parametric = false, bool boomerang = false, float amplitude = 0, float frequency = 1,
        float magnitude = 3, int size = 100) {
        var props = new ProjectileProps(objectId, lifetimeMs,
            speed, damage, minDamage, maxDamage, effects, multiHit, passesCover,
            armorPiercing, wavy, parametric, boomerang, amplitude, frequency, magnitude, size);

        // Check if projectile doesn't exist already
        foreach (var kvp in _dict) {
            var p = kvp.Value;
            if (p.Props.Equals(props)) // Return the existing match
                return p;
        }

        var ret = new ContainerProjectileProps(_nextProjId++, props);

        _customDict.TryAdd(ret.ProjectileIndex, ret);
        return ret;
    }
}