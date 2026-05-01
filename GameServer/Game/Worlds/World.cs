using Common.Game;
using Common.Resources.World;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    
    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly EntityManager Entities;
    
    public MapData Map;
    public string DisplayName;
    public string Music;

    public bool Deleted;

    public World(int id, WorldConfig config) {
        Id = id;
        Config = config;
        Entities = new EntityManager(this);

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
    }

    public void Tick(ref RealmTime time) {
        Console.WriteLine($"[{Id}] Entities:{Entities.Count} | Delta:{time.ElapsedMsDelta}");
    }
}