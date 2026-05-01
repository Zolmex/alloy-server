using Common.Game.Objects;
using Common.Resources.World;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    
    public readonly int Id;
    public readonly WorldConfig Config;

    public readonly LazyCollection<Entity> Entities = new();
    
    public MapData Map;

    private int _nextEntityId;

    public World(int id, WorldConfig config) {
        Id = id;
        Config = config;
    }

    public void Load(int mapId) {
        Map = WorldLibrary.MapDatas[Config.Name][mapId];
        LoadEntities();
    }

    public void LoadEntities() {
        foreach (var orig in Map.Entities) {
            var en = Entity.Resolve(orig.Desc.ObjectType);
            en.Move(orig.Pos.X, orig.Pos.Y);
            EnterWorld(en);
        }
    }

    public void EnterWorld(Entity en) {
        en.Id = Interlocked.Increment(ref _nextEntityId);
        Entities.Add(en);
    }

    public void Update() {
        Entities.Update();
    }

    public void Tick(ref RealmTime time) {
        Console.WriteLine($"[{Id}] Entities:{Entities.Count} | Delta:{time.ElapsedMsDelta}");
    }
}