using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Common.Database.Models;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
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
    public readonly EventSystem EventSystem;
    public readonly DamageSystem DamageSystem;
    public readonly ChatSystem ChatSystem;
    public readonly BehaviorSystem BehaviorSystem;
    public readonly ProjectileSystem ProjectileSystem;

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
        EventSystem = new EventSystem(this);
        DamageSystem = new DamageSystem(this);
        ChatSystem = new ChatSystem(this);
        BehaviorSystem = new BehaviorSystem(this);
        ProjectileSystem = new ProjectileSystem(this);

        Load(mapId);
        
        StatsSystem.Initialize();
        InventorySystem.Initialize();
        PlayerSightSystem.Initialize();
        EventSystem.Initialize();
        DamageSystem.Initialize();
        ChatSystem.Initialize();
        BehaviorSystem.Initialize();
        ProjectileSystem.Initialize();
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
            InitEntity(en, desc, orig.Pos);
        }
    }

    public void Update() { // Runs in-between ticks
        while (_removeEntities.TryDequeue(out var en)) {
            if (Ecs.IsAlive(en)) { // Needed for ID + version safety
                DestroyEntity(en);
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
        DamageSystem.Tick(ref time);
        ProjectileSystem.Tick(ref time);
        BehaviorSystem.Tick(ref time);
        PlayerSightSystem.Tick(ref time);
        StatsSystem.Tick(ref time);
        ChatSystem.Tick(ref time);
    }

    public Entity EnterPlayer(ushort objType, User user) {
        var en = EnterWorld(XmlLibrary.ObjectDescs[objType]);
        InitPlayer(en, user.Session.Account, user.Session.Char);
        Users = Users.Add(en, user);
        return en;
    }

    public Entity EnterWorld(ObjectDesc desc) {
        var en = Create(desc);

        if (Ecs.Has<Combat>(en))
            DamageSystem.AddRecord(en);
        if (Ecs.Has<Behavior>(en))
            BehaviorSystem.Add(en, desc);
        if (Ecs.Has<PlayerTag>(en))
            PlayerSightSystem.Add(en);
        
        if (!_entities.TryAdd(new EntityId(en), en))
            throw new Exception($"Entity {en.Id}[{en.Version}]('{desc.ObjectId}') already exists.");
        return en;
    }

    private Entity Create(ObjectDesc desc) {
        if (desc.Class != null)
            switch (desc.Class) {
                case "Projectile":
                    return Ecs.Create(
                        new ProjectileTag(), // You can add components to projectiles here if you want :)
                        new ObjectType(desc.ObjectType)
                        );
                case "ConnectedWall":
                case "CaveWall":
                case "Wall":
                    return CreateStaticObject(desc);
                case "Portal":
                case "GuildHallPortal":
                    return Ecs.Create(
                        new PortalTag(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "Character":
                    if (desc.Enemy)
                        return CreateEnemy(desc);
                    return Ecs.Create(
                        new CharacterTag(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position(),
                        new Behavior(),
                        new Combat()
                        );
                case "ClosedVaultChest":
                case "Container":
                    var containerDesc = XmlLibrary.ContainerDescs[desc.ObjectType];
                    return Ecs.Create(
                        new ContainerTag(),
                        new ObjectType(desc.ObjectType),
                        new Stats(desc),
                        new Flags(),
                        new Position(),
                        new Inventory(containerDesc.SlotTypes),
                        new Behavior()
                        );
                case "Merchant":
                case "GuildMerchant":
                    return Ecs.Create(
                        new MerchantTag(),
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
            new PlayerTag(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position(),
            new Inventory(playerDesc.SlotTypes),
            new PlayerChat(),
            new Combat()
        );
    }
    
    private Entity CreateEnemy(ObjectDesc desc) {
        return Ecs.Create(
            new EnemyTag(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position(),
            new Behavior(),
            new Combat()
        );
    }

    private Entity CreateStaticObject(ObjectDesc desc) {
        return Ecs.Create(
            new StaticObjectTag(),
            new ObjectType(desc.ObjectType),
            new Stats(desc),
            new Flags(),
            new Position(),
            new Behavior(),
            new Combat()
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
    
    public virtual World GetInstance(User user) {
        return this;
    }
    
    private void InitEntity(Entity en, ObjectDesc desc, WorldPosData spawnPos) {
        ref var pos = ref Ecs.Get<Position>(en);
        pos.Move(spawnPos.X, spawnPos.Y);

        if (desc.Static) {
            var tile = Map[(int)spawnPos.X, (int)spawnPos.Y];
            if (tile.Object == Entity.Null)
                tile.Object = en;
        }
    }
    
    private void InitPlayer(Entity player, Account acc, Character chr) {
        ref var stats = ref Ecs.Get<Stats>(player);
        ref var inv = ref Ecs.Get<Inventory>(player);
        stats.InitPlayer(acc, chr);
        inv.InitPlayer(acc, chr);
        
        var spawnTile = Map.Data.Regions[TileRegion.Spawn].RandomElement();
        ref var pos = ref Ecs.Get<Position>(player);
        pos.Move(spawnTile.X, spawnTile.Y);
    }

    private void DestroyEntity(Entity en) { // This is the deallocation part, use LeaveWorld to kill an entity
        Ecs.Destroy(en);
        _entities.Remove((EntityId)en, out _);

        DamageSystem.RemoveRecord(en);
        PlayerSightSystem.Remove(en);
        BehaviorSystem.Remove(en);
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
}