using System.Collections.Concurrent;
using System.Collections.Immutable;
using Common.Game;
using Common.Resources.World;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    public const int UNBLOCKED_SIGHT = 0;

    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly EntityManager Entities;
    public readonly EntityStatsManager EntityStats;
    public readonly PlayerSightManager PlayerSights;
    public readonly PlayerChatManager PlayerChat;

    public ImmutableDictionary<int, User> PlayerToUser;

    public MapData Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    private readonly ConcurrentQueue<Action> _pendingActions = [];

    public World(int id, int mapId, WorldConfig config) {
        Id = id;
        Config = config;

        Entities = new EntityManager(5_000);
        EntityStats = new EntityStatsManager(this, 5_000);
        PlayerSights = new PlayerSightManager(this, 100);
        PlayerChat = new PlayerChatManager(this, 100);

        PlayerToUser = ImmutableDictionary<int, User>.Empty;

        DisplayName = config.DisplayName;
        Music = config.Music;

        Load(mapId);
    }

    public void Load(int mapId) {
        Map = WorldLibrary.MapDatas[Config.Name][mapId];
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Entities) {
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
        var stats = new EntityStats(ref en);
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
                break;
            case EntityType.Enemy:
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
        EntityStats.Remove(entityId);
        PlayerSights.Remove(entityId);
    }

    public void Enqueue(Action act) {
        _pendingActions.Enqueue(act);
    }

    public void Tick(ref RealmTime time) {
        while (_pendingActions.TryDequeue(out var act))
            act();
        
        PlayerSights.Tick(ref time);
        EntityStats.Tick(ref time); // Clears stat update masks, needs to happen AFTER player update
    }
}