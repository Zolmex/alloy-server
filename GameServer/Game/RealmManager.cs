using System.Collections.Immutable;
using System.Diagnostics;
using Common.Database.Models;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Worlds;
using GameServer.Game.Worlds.Logic;

namespace GameServer.Game;

public class RealmManager {
    private static readonly Logger _log = new(typeof(RealmManager));

    public static ImmutableDictionary<int, World> Worlds = ImmutableDictionary.Create<int, World>();
    public static ImmutableDictionary<int, User> Users = ImmutableDictionary.Create<int, User>();
    public static ImmutableDictionary<int, Account> Accounts = ImmutableDictionary.Create<int, Account>();
    
    public static void Init() {
        AddWorld(World.NEXUS_ID, 0, new Nexus());
        
        _log.Info("Realm Manager initialized.");
    }

    public static void AddWorld(int id, int mapId, World world) {
        Worlds = Worlds.Add(id, world);
        world.Load(mapId);
    }

    public static void UserConnected(User user) {
        Users = Users.Add(user.Id, user);
        user.StartNetwork();
        _log.Debug($"User {user.Id} connected from {user.Network.IP}");
    }
    
    public static void UserDisconnected(User user) {
        Users = Users.Remove(user.Id);
    }
}