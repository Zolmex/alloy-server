using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using GameServer.Utilities;
using ArchWorld = Arch.Core.World;
using Entity = Arch.Core.Entity;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    public const int UNBLOCKED_SIGHT = 0;
    public const int LINE_OF_SIGHT = 1;

    public int Id;
    public readonly int MapId;
    public readonly WorldConfig Config;
    public readonly ArchWorld Ecs = ArchWorld.Create();

    public readonly List<string> TextCache = [];
    public ImmutableDictionary<Entity, User> Users;

    public WorldMap Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    private readonly List<(long Delay, Action<World> Action)> _timedActions = [];
    private readonly ConcurrentQueue<Entity> _removeEntities = [];
    private readonly Dictionary<EntityId, Entity> _entities = []; 
    
    public readonly StatsSystem StatsSystem;
    public readonly InventorySystem InventorySystem;
    public readonly PlayerSightSystem PlayerSightSystem;

    public World(int id, int mapId, WorldConfig config) {
        Id = id;
        MapId = mapId;
        Config = config;
        DisplayName = config.DisplayName;
        Music = config.Music;
        Users = ImmutableDictionary<Entity, User>.Empty;
        
        StatsSystem = new StatsSystem(this);
        InventorySystem = new InventorySystem(this);
        PlayerSightSystem = new PlayerSightSystem(this);

        Load(mapId);
        
        StatsSystem.Initialize();
        InventorySystem.Initialize();
        PlayerSightSystem.Initialize();
    }

    public void Load(int mapId) {
        var maps = WorldLibrary.MapDatas[Config.Name];
        if (mapId == -1)
            mapId = Random.Shared.Next(maps.Length - 1);
        
        Map = new WorldMap(this, maps[mapId]);
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Data.Entities) {
            var desc = XmlLibrary.ObjectDescs[orig.ObjType];
            var en = EnterWorld(desc);
            ref var pos = ref en.Get<Position>();
            pos.Init(this, orig.Pos);
            if (desc.Static) {
                var tile = Map[(int)orig.Pos.X, (int)orig.Pos.Y];
                if (tile.Object == Entity.Null)
                    tile.Object = en;
            }
        }
    }

    public void Update() { // Runs in-between ticks
        while (_removeEntities.TryDequeue(out var en)) {
            if (Ecs.IsAlive(en)) { // Needed for ID safety
                Ecs.Destroy(en);
                _entities.Remove(new EntityId(en), out _);
            }
        }
    }
    
    public void Tick(ref RealmTime time) {
        HandleTimers();

        Map.Tick(ref time);
        
        // PortalDatas.Tick(ref time);
        // EntityCombat.Tick(ref time);
        // EntityProjectiles.Tick(ref time);
        // EntityBehaviors.Tick(ref time);
        // PlayerSights.Tick(ref time);
        // EntityStats.Tick(ref time);
        
        InventorySystem.Tick(ref time);
        InventorySystem.ProcessQuery(Ecs);
        PlayerSightSystem.ProcessQuery(Ecs, ref time);
        StatsSystem.TickQuery(Ecs, ref time);
        
        ClearTextCache();
    }

    public Entity EnterPlayer(ushort objType, User user) {
        var en = EnterWorld(XmlLibrary.ObjectDescs[objType]);
        Users = Users.Add(en, user);
        return en;
    }

    public Entity EnterWorld(ObjectDesc desc) {
        var en = Create(desc);
        if (!_entities.TryAdd(new EntityId(en), en))
            throw new Exception($"Entity {en.Id}[{en.Version}]('{desc.ObjectId}') already exists.");
        return en;
    }

    private Entity Create(ObjectDesc desc) {
        if (desc.Class != null)
            switch (desc.Class) {
                case "Projectile":
                    return Ecs.Create(
                        new ProjectileType(), // You can add components to projectiles here if you want :)
                        new ObjectType(desc.ObjectType)
                        );
                case "ConnectedWall":
                case "CaveWall":
                case "Wall":
                    return CreateStaticObject(desc);
                case "Portal":
                case "GuildHallPortal":
                    return Ecs.Create(
                        new PortalType(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "Character":
                    if (desc.Enemy)
                        return CreateEnemy(desc);
                    return Ecs.Create(
                        new CharacterType(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "ClosedVaultChest":
                case "Container":
                    var containerDesc = XmlLibrary.ContainerDescs[desc.ObjectType];
                    return Ecs.Create(
                        new ContainerType(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position(),
                        new Inventory(containerDesc.SlotTypes)
                        );
                case "Merchant":
                case "GuildMerchant":
                    return Ecs.Create(
                        new MerchantType(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
            }

        if (desc.Enemy)
            return CreateEnemy(desc);
        
        if (desc.Static)
            return CreateStaticObject(desc);

        if (desc.Player)
            return CreatePlayer(desc);

        return Ecs.Create(new Stats(desc));
    }
    
    private Entity CreatePlayer(ObjectDesc desc) {
        var playerDesc = XmlLibrary.PlayerDescs[desc.ObjectType];
        return Ecs.Create(
            new PlayerType(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position(),
            new Inventory(playerDesc.SlotTypes)
        );
    }
    
    private Entity CreateEnemy(ObjectDesc desc) {
        return Ecs.Create(
            new EnemyType(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position()
        );
    }

    private Entity CreateStaticObject(ObjectDesc desc) {
        return Ecs.Create(
            new StaticObjectType(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position()
            );
    }

    public void LeaveWorld(Entity en) {
        if (en == Entity.Null)
            return;
        
        _removeEntities.Enqueue(en);
        Users = Users.Remove(en);
    }

    public Entity GetEntity(EntityId id) {
        if (!_entities.TryGetValue(id, out var entity)) {
            return Entity.Null;
        }
        return entity;
    }
    
    public void AddTimedAction(int time, Action<World> act) {
        _timedActions.Add((GameLogic.WorldTime.TickCount + TimeUtils.TicksFromTime(time, GameLogic.TPS), act));
    }
    
    public void PlayerText(string text) {
        TextCache.Add(text);
    }

    public virtual World GetInstance(User user) {
        return this;
    }
    
    private void HandleTimers() {
        for (var i = 0; i < _timedActions.Count; i++) {
            var timer = _timedActions[i];
            if (timer.Delay <= GameLogic.WorldTime.TickCount) {
                timer.Action(this);
                _timedActions.RemoveAt(i);
                i--;
            }
        }
    }

    private void ClearTextCache() {
        TextCache.Clear();
    }
}