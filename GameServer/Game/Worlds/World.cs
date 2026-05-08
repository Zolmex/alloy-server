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
    
    public readonly EntityBehaviorManager EntityBehaviors;
    public readonly EntityStatsManager EntityStats;
    
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

        Entities = new EntityManager(5_000);
        
        EntityBehaviors = new EntityBehaviorManager(this, 5_000);
        EntityStats = new EntityStatsManager(this, 5_000);
        
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
        AddComponents(ref en);
        return ref ret;
    }

    private void AddComponents(ref Entity en) {
        var stats = new EntityStats(this, ref en);
        EntityStats.Add(ref stats, en.Id); // All entities must have

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
                var behavior = new EntityBehavior(this, ref en);
                ref var entityBehavior = ref EntityBehaviors.Add(ref behavior, en.Id);
                if (BehaviorLibrary.ClassicBehaviors.TryGetValue(en.Desc.ObjectId, out var rootState))
                    entityBehavior.Load(rootState);
                break;
            case EntityType.Container:
                break;
            case EntityType.Player:
                var sight = new PlayerSight(ref en);
                PlayerSights.Add(ref sight, en.Id);
                var chat = new PlayerChat(this, ref en);
                PlayerChat.Add(ref chat, en.Id);
                break;
            default:
                throw new ArgumentOutOfRangeException($"{en.Type}");
        }
    }

    public void LeaveWorld(int entityId) {
        Entities.Remove(entityId);
        EntityBehaviors.Remove(entityId);
        EntityStats.Remove(entityId);
        PlayerSights.Remove(entityId);
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

        Map.Tick(ref time);
        
        EntityBehaviors.Tick(ref time);
        PlayerSights.Tick(ref time);
        EntityStats.Tick(ref time); // Clears stat update masks, needs to happen AFTER player update
        
        ClearTextCache();
    }
}