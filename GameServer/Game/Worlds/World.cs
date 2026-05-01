namespace GameServer.Game.Worlds;

public class World {

    public const int NEXUS_ID = -1;
    public const int TEST_ID = -2;
    
    public readonly int Id;

    public World(int id) {
        Id = id;
    }

    public void Tick() {
        Console.WriteLine($"[{Id}] TICK");
    }
}