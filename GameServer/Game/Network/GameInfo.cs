using Common.Database.Models;
using Common.Game;
using Common.Utilities;
using GameServer.Game.Entities;
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
    public int PlayerId;

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
        PlayerId = world.EnterPlayer(ref plr, User);
    }

    public void Unload(bool reconnect, bool death) {
        State = GameState.Idle;
        if (PlayerId == 0 || death)
            return;
        
        World.LeavePlayer(PlayerId);
        PlayerId = 0;
    }

    public void Reset() {
        State = GameState.Idle; // Change our state first
        World = null;
        Char = null;
        PlayerId = 0;
    }
}