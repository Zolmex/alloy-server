using Common.Database.Models;
using Common.Game;
using Common.Utilities;
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
    public World World;
    public Character Char;
    public Entity Player;

    public GameInfo(User user) {
        User = user;
    }

    public GameState State { get; private set; }

    public void SetWorld(World world) {
        State = GameState.Loading;
        World = world;
    }

    public void Load(Character chr, World world) {
        State = GameState.Playing;
        // Save player and character info, enter player to world
    }

    public void Unload(bool reconnect, bool death) {
        State = GameState.Idle;
        // Save character and leave world
    }

    public void Reset() {
        State = GameState.Idle; // Change our state first
        World = null;
    }
}