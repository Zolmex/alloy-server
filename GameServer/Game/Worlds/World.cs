using Common.Game;
using Common.Game.Components;
using Common.Game.Systems;
using Common.Resources.World;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    
    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly EntityManager Entities;
    public readonly EntityStatsManager EntityStats;
    
    public MapData Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    public World(int id, WorldConfig config) {
        Id = id;
        Config = config;
        Entities = new EntityManager(config.Name == "Realm" ? 50_000 : 5_000);
        EntityStats = new EntityStatsManager(config.Name == "Realm" ? 50_000 : 5_000);

        DisplayName = config.DisplayName;
        Music = config.Music;
    }

    public void Load(int mapId) {
        Map = WorldLibrary.MapDatas[Config.Name][mapId];
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Entities) {
            var en = new Entity(orig.Desc.ObjectType);
            en.Move(orig.Pos.X, orig.Pos.Y);
            EnterWorld(ref en);
        }
    }

    public void EnterWorld(ref Entity en) {
        Entities.Add(ref en);
        var stats = new StatsComponent();
        EntityStats.Add(ref stats, en.Id);
    }

    public void LeaveWorld(int entityId) {
        Entities.Remove(entityId);
        EntityStats.Remove(entityId);
    }

    public void Tick(ref RealmTime time) {
        EntityStats.Tick(ref time);
    }
}