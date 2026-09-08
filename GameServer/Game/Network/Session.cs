using Arch.Core;
using Common.Database.Models;
using Common.Game;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Extensions;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Network;

public enum GameState {
    Idle, // User has established connection to server but hasn't loaded to any world yet
    Loading, // User has sent Hello packet, and now we're waiting for client to send Load packet
    Playing // User has established
}

public class Session {
    private static readonly Logger _log = new(typeof(Session));

    public readonly User User;
    public Account Account;
    public World World;
    public Character Char;
    public Entity Player;
    
    public SessionDto Data => new(Account.Id, World.Id, World.DisplayName, World.Ecs.Get<Position>(Player).Pos);

    public Session(User user) {
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
        
        var newPlr = world.EnterPlayer(chr.ObjectType, User);
        newPlr.InitPlayer(world, Account, Char);
        newPlr.MoveToSpawn(world);
        
        Player = newPlr;
    }

    public void Unload() {
        ref var inv = ref World.EntityInventories.Get(PlayerId);
        if (inv.Id != EntityId.Null)
            inv.Save(Char);
        
        World?.LeaveWorld(PlayerId);
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