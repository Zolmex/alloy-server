using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Entities.Projectiles;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using GameServer.Utilities;

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
    public ImmutableDictionary<EntityId, User> Users;

    public WorldMap Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    private readonly List<(long Delay, Action<World> Action)> _timedActions = [];
    private readonly ConcurrentQueue<EntityId> _removeEntities = [];

    public World(int id, int mapId, WorldConfig config) {
        Id = id;
        MapId = mapId;
        Config = config;

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

        Users = ImmutableDictionary<EntityId, User>.Empty;

        DisplayName = config.DisplayName;
        Music = config.Music;

        Load(mapId);
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
            var en = new Entity(orig.ObjType);
            ref var newEn = ref EnterWorld(ref en);
            newEn.Init(this, orig.Pos);
        }
    }

    public ref Entity EnterPlayer(ref Entity en, User user) {
        ref var ret = ref EnterWorld(ref en);
        Users = Users.Add(ret.Id, user);
        return ref ret;
    }

    public ref Entity EnterWorld(ref Entity en) {
        ref var ret = ref Entities.Add(ref en);
        AddComponents(ref ret);
        return ref ret;
    }

    private void AddComponents(ref Entity en) {
        var stats = new EntityStats(this, ref en);
        EntityStats.Add(ref stats); // All entities must have

        switch (en.Type) {
            case EntityType.GameObject:
                break;
            case EntityType.StaticObject:
                break;
            case EntityType.Portal:
                var portalData = new PortalData(this, ref en);
                PortalDatas.Add(ref portalData);
                break;
            case EntityType.Merchant:
                break;
            case EntityType.Character:
            case EntityType.Enemy:
                var events = new EntityEvents(this, ref en);
                EntityEvents.Add(ref events);
                var behavior = new EntityBehavior(this, ref en);
                behavior.Load();
                EntityBehaviors.Add(ref behavior);
                var enProjectiles = new EntityProjectiles(this, ref en);
                EntityProjectiles.Add(ref enProjectiles);
                var combat = new EntityCombat(this, ref en);
                EntityCombat.Add(ref combat);
                break;
            case EntityType.Container:
                var desc = XmlLibrary.ContainerDescs[en.Desc.ObjectType];
                var inv = new EntityInventory(this, ref en, 8);
                inv.Init(desc.SlotTypes, []);
                EntityInventories.Add(ref inv);
                break;
            case EntityType.Player:
                var slotTypes = XmlLibrary.PlayerDescs[en.Desc.ObjectType].SlotTypes;
                inv = new EntityInventory(this, ref en, 20);
                inv.Init(slotTypes, []);
                EntityInventories.Add(ref inv);
                events = new EntityEvents(this, ref en);
                EntityEvents.Add(ref events);
                var sight = new PlayerSight(this, ref en);
                PlayerSights.Add(ref sight);
                var chat = new PlayerChat(this, ref en);
                PlayerChat.Add(ref chat);
                enProjectiles = new EntityProjectiles(this, ref en);
                EntityProjectiles.Add(ref enProjectiles);
                combat = new EntityCombat(this, ref en);
                EntityCombat.Add(ref combat);
                break;
            default:
                throw new ArgumentOutOfRangeException($"{en.Type}");
        }
    }

    public void LeaveWorld(EntityId entityId) {
        _removeEntities.Enqueue(entityId);
    }
    
    private void RemoveEntity(EntityId entityId) {
        EntityEvents.Remove(entityId); // First to go is events, so DeathEvent gets called before getting removed from the rest of component managers
        Entities.Remove(entityId);
        EntityBehaviors.Remove(entityId);
        EntityCombat.Remove(entityId);
        EntityStats.Remove(entityId);
        EntityProjectiles.Remove(entityId);
        EntityInventories.Remove(entityId);
        PortalDatas.Remove(entityId);
        PlayerSights.Remove(entityId);
        PlayerChat.Remove(entityId);
        Users = Users.Remove(entityId);
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
        while (_removeEntities.TryDequeue(out var entityId))
            RemoveEntity(entityId);
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
        
        ClearTextCache();
    }
}