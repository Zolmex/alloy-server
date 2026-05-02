using Common.Game;
using Common.Resources.World;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Systems;
using GameServer.Game.Network;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    
    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly EntityManager Entities;
    public readonly EntityStatsManager EntityStats;
    public readonly PlayerSightManager PlayerSights;
    
    public MapData Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    public World(int id, WorldConfig config) {
        Id = id;
        Config = config;
        Entities = new EntityManager(5_000);
        EntityStats = new EntityStatsManager(5_000);
        PlayerSights = new PlayerSightManager(100);

        DisplayName = config.DisplayName;
        Music = config.Music;
    }

    public void Load(int mapId) {
        Map = WorldLibrary.MapDatas[Config.Name][mapId];
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Entities) {
            var en = new Entity(orig.ObjType);
            en.Move(orig.Pos.X, orig.Pos.Y);
            EnterWorld(ref en);
        }
    }

    public int EnterWorld(ref Entity en) {
        Entities.Add(ref en);
        AddComponents(ref en);
        return en.Id;
    }

    private void AddComponents(ref Entity en) {
        var stats = new StatsComponent();
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
                var sight = new PlayerSightComponent();
                PlayerSights.Add(ref sight, en.Id);
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

    public void Tick(ref RealmTime time) {
        EntityStats.Tick(ref time);
        PlayerSights.Tick(ref time);
    }
}