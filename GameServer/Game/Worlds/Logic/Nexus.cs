using Common.Resources.World;

namespace GameServer.Game.Worlds.Logic;

public class Nexus : World {

    public Nexus() : base(NEXUS_ID, 0, WorldLibrary.WorldConfigs["Nexus"]) {
        
    }
}