using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Common.Game;
using Common.Resources.World;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;
using GameServer.Game.Entities.Projectiles;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;
using GameServer.Utilities;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    public const int UNBLOCKED_SIGHT = 0;

    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly EntityManager Entities;
    public readonly ProjectileManager Projectiles;
    
    public readonly EntityBehaviorManager EntityBehaviors;
    public readonly EntityStatsManager EntityStats;
    public readonly EntityProjectilesManager EntityProjectiles;
    public readonly EntityCombatManager EntityCombat;
    public readonly EntityEventsManager EntityEvents;
    
    public readonly PlayerSightManager PlayerSights;
    public readonly PlayerChatManager PlayerChat;

    public ImmutableDictionary<int, User> PlayerToUser;
    public ImmutableList<string> TextCache;

    public WorldMap Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    private readonly ConcurrentQueue<Action> _pendingActions = [];
    private readonly List<(long Delay, Action<World> Action)> _timedActions = [];

    public World(int id, int mapId, WorldConfig config) {
        Id = id;
        Config = config;

        Entities = new EntityManager(this, 5_000);
        Projectiles = new ProjectileManager(this, 5_000);
        
        EntityBehaviors = new EntityBehaviorManager(this, 5_000);
        EntityStats = new EntityStatsManager(this, 5_000);
        EntityProjectiles = new EntityProjectilesManager(this, 1_000);
        EntityCombat = new EntityCombatManager(this, 1_000);
        EntityEvents = new EntityEventsManager(this, 1_000);
        
        PlayerSights = new PlayerSightManager(this, 100);
        PlayerChat = new PlayerChatManager(this, 100);

        PlayerToUser = ImmutableDictionary<int, User>.Empty;
        TextCache = ImmutableList<string>.Empty;

        DisplayName = config.DisplayName;
        Music = config.Music;

        Load(mapId);
    }

    public void Load(int mapId) {
        Map = new WorldMap(this, WorldLibrary.MapDatas[Config.Name][mapId]);
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Data.Entities) {
            var en = new Entity(orig.ObjType);
            EnterWorld(ref en);
            ref var stats = ref EntityStats.Get(en.Id);
            stats.Move(orig.Pos.X, orig.Pos.Y);
            var tile = Map[(int)orig.Pos.X, (int)orig.Pos.Y];
            tile.ObjectId = en.Id;
        }
    }

    public ref Entity EnterPlayer(ref Entity en, User user) {
        ref var ret = ref EnterWorld(ref en);
        PlayerToUser = PlayerToUser.Add(en.Id, user);
        return ref ret;
    }

    public void LeavePlayer(int id) {
        LeaveWorld(id);
        PlayerToUser = PlayerToUser.Remove(id);
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
                break;
            case EntityType.Merchant:
                break;
            case EntityType.Character:
            case EntityType.Enemy:
                var events = new EntityEvents(this, ref en);
                EntityEvents.Add(ref events);
                var behavior = new EntityBehavior(this, ref en);
                ref var entityBehavior = ref EntityBehaviors.Add(ref behavior);
                if (BehaviorLibrary.ClassicBehaviors.TryGetValue(en.Desc.ObjectId, out var rootState))
                    entityBehavior.Load(rootState);
                var enProjectiles = new EntityProjectiles(this, ref en);
                EntityProjectiles.Add(ref enProjectiles);
                var combat = new EntityCombat(this, ref en);
                EntityCombat.Add(ref combat);
                break;
            case EntityType.Container:
                break;
            case EntityType.Player:
                events = new EntityEvents(this, ref en);
                EntityEvents.Add(ref events);
                var sight = new PlayerSight(ref en);
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

    public void LeaveWorld(int entityId) {
        EntityEvents.Remove(entityId); // First to go is events, so DeathEvent gets called before getting removed from the rest of component managers
        Entities.Remove(entityId);
        EntityBehaviors.Remove(entityId);
        EntityCombat.Remove(entityId);
        EntityStats.Remove(entityId);
        EntityProjectiles.Remove(entityId);
        PlayerSights.Remove(entityId);
        PlayerChat.Remove(entityId);
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
    
    public void Enqueue(Action act) {
        _pendingActions.Enqueue(act);
    }

    public void PlayerText(string text) {
        TextCache = TextCache.Add(text);
    }

    private void ClearTextCache() {
        TextCache = TextCache.Clear();
    }

    public void Tick(ref RealmTime time) {
        while (_pendingActions.TryDequeue(out var act))
            act();

        HandleTimers();

        Projectiles.Tick(ref time);
        Map.Tick(ref time);
        
        EntityStats.Tick(ref time);
        EntityCombat.Tick(ref time);
        EntityProjectiles.Tick(ref time);
        EntityBehaviors.Tick(ref time);
        PlayerSights.Tick(ref time);
        
        ClearTextCache();
    }
}