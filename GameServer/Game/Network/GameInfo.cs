using Common.Database.Models;
using Common.Game;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Worlds;

namespace GameServer.Game.Network;

public enum GameState {
    Idle, // User has established connection to server but hasn't loaded to any world yet
    Loading, // User has sent Hello packet, and now we're waiting for client to send Load packet
    Playing // User has established
}

public class GameInfo {
    private static readonly Logger _log = new(typeof(GameInfo));

    public readonly User User;
    public Account Account;
    public World World;
    public Character Char;
    public EntityId PlayerId;
    
    public ref Entity Player => ref World.Entities.Get(PlayerId);

    public GameInfo(User user) {
        User = user;
    }

    public GameState State { get; private set; }

    public void SetWorld(Account acc, World world) {
        Account = acc;
        State = GameState.Loading;
        World = world;
    }

    public void Load(Character chr, World world) {
        State = GameState.Playing;
        Char = chr;
        
        var plr = new Entity(chr.ObjectType);
        ref var newPlr = ref world.EnterPlayer(ref plr, User);
        newPlr.InitPlayer(User, world, Account, Char);
        newPlr.MoveToSpawn(world);
        
        PlayerId = newPlr.Id;
    }

    public void Unload() {
        State = GameState.Idle;
        PlayerId = EntityId.Null;
    }

    public void Reset() {
        State = GameState.Idle; // Change our state first
        World = null;
        Char = null;
        PlayerId = EntityId.Null;
    }
}