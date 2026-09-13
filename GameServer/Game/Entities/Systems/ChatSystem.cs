using Arch.System;
using Common.Game;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Systems;

public class ChatSystem(World world) : BaseSystem<World, RealmTime>(world) {
    
    public readonly List<string> TextCache = [];
    
    // TODO: When player speaks, add to TextCache

    public void Tick(ref RealmTime time) {
        TextCache.Clear();
    }
}