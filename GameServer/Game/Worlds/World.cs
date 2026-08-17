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
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Behaviors;
using GameServer.Game.Entities.Old.Components;
using GameServer.Game.Entities.Old.Events;
using GameServer.Game.Entities.Old.Extensions;
using GameServer.Game.Entities.Old.Projectiles;
using GameServer.Game.Entities.Old.Systems;
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

    public readonly EntityManager Entities;
    public readonly ProjectileManager Projectiles;
    
    public readonly EntityBehaviorManager EntityBehaviors;
    public readonly EntityStatsManager EntityStats;
    public readonly EntityProjectilesManager EntityProjectiles;
    public readonly EntityCombatManager EntityCombat;
    public readonly EntityEventsManager EntityEvents;
    public readonly EntityInventoryManager EntityInventories;
    public readonly PortalDatasManager PortalDatas;
    
    public readonly PlayerSightManager PlayerSights;
    public readonly PlayerChatManager PlayerChat;

    public readonly List<string> TextCache = [];
    public ImmutableDictionary<int, User> Users;

    public WorldMap Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    private readonly List<(long Delay, Action<World> Action)> _timedActions = [];
    private readonly ConcurrentQueue<Entity> _removeEntities = [];
    
    private readonly ArchWorld _archWorld = ArchWorld.Create();
    private readonly StatsSystem _statsSystem;

    public World(int id, int mapId, WorldConfig config) {
        Id = id;
        MapId = mapId;
        Config = config;
        
        _statsSystem = new StatsSystem(_archWorld);
        
        Entities = new EntityManager(this, 5_000);
        Projectiles = new ProjectileManager(this, 5_000);
        
        EntityBehaviors = new EntityBehaviorManager(this, 5_000);
        EntityStats = new EntityStatsManager(this, 5_000);
        EntityProjectiles = new EntityProjectilesManager(this, 1_000);
        EntityCombat = new EntityCombatManager(this, 1_000);
        EntityEvents = new EntityEventsManager(this, 1_000);
        EntityInventories = new EntityInventoryManager(this, 1_000);
        PortalDatas = new PortalDatasManager(this, 1_000);
        
        PlayerSights = new PlayerSightManager(this, 100);
        PlayerChat = new PlayerChatManager(this, 100);

        Users = ImmutableDictionary<int, User>.Empty;

        DisplayName = config.DisplayName;
        Music = config.Music;

        Load(mapId);
        _statsSystem.Initialize();
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
            var en = Create(desc);
            ref var pos = ref en.Get<Position>();
            pos.Init(this, orig.Pos);
            if (desc.Static) {
                var tile = Map[(int)orig.Pos.X, (int)orig.Pos.Y];
                if (tile.ObjectId == -1)
                    tile.ObjectId = en.Id;
            }
        }
    }

    public Entity EnterPlayer(ushort objType, User user) {
        var en = Create(XmlLibrary.ObjectDescs[objType]);
        Users = Users.Add(en.Id, user);
        return en;
    }

    private Entity Create(ObjectDesc desc) {
        if (desc.Class != null)
            switch (desc.Class) {
                case "ConnectedWall":
                case "CaveWall":
                case "Wall":
                    return CreateStaticObject(desc);
                case "Portal":
                case "GuildHallPortal":
                    return _archWorld.Create(
                        new Portal(),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "Character":
                    if (desc.Enemy)
                        return CreateEnemy(desc);
                    return _archWorld.Create(
                        new Character(),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "ClosedVaultChest":
                case "Container":
                    return _archWorld.Create(
                        new Container(),
                        new Stats(desc),
                        new Flags(),
                        new Position()
                        );
                case "Merchant":
                case "GuildMerchant":
                    return _archWorld.Create(
                        new Merchant(),
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

        return _archWorld.Create(new Stats(desc));
    }
    
    private Entity CreatePlayer(ObjectDesc desc) {
        return _archWorld.Create(
            new Player(),
            new Stats(desc),
            new Flags(),
            new Position()
        );
    }
    
    private Entity CreateEnemy(ObjectDesc desc) {
        return _archWorld.Create(
            new Enemy(),
            new Stats(desc),
            new Flags(),
            new Position()
        );
    }

    private Entity CreateStaticObject(ObjectDesc desc) {
        return _archWorld.Create(
            new StaticObject(),
            new Stats(desc),
            new Flags(),
            new Position()
            );
    }

    public void LeaveWorld(Entity en) {
        _removeEntities.Enqueue(en);
        Users = Users.Remove(en.Id);
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

    public void AddTimedAction(int time, Action<World> act) {
        _timedActions.Add((GameLogic.WorldTime.TickCount + TimeUtils.TicksFromTime(time, GameLogic.TPS), act));
    }
    
    public void PlayerText(string text) {
        TextCache.Add(text);
    }

    private void ClearTextCache() {
        TextCache.Clear();
    }

    public void Update() { // Runs in-between ticks
        while (_removeEntities.TryDequeue(out var en)) {
            if (_archWorld.IsAlive(en)) // Needed for ID safety
                _archWorld.Destroy(en);
        }
    }

    public virtual World GetInstance(User user) {
        return this;
    }
    
    public void Tick(ref RealmTime time) {
        HandleTimers();

        Projectiles.Tick(ref time);
        Map.Tick(ref time);
        
        PortalDatas.Tick(ref time);
        EntityInventories.Tick(ref time);
        EntityCombat.Tick(ref time);
        EntityProjectiles.Tick(ref time);
        EntityBehaviors.Tick(ref time);
        PlayerSights.Tick(ref time);
        EntityStats.Tick(ref time);
        
        _statsSystem.UpdateStatsQuery(_archWorld, ref time);
        
        ClearTextCache();
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
}