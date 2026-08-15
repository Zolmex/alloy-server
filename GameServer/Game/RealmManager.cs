using System.Collections.Immutable;
using System.Diagnostics;
using Common.Database.Models;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using GameServer.Game.Worlds.Logic;

namespace GameServer.Game;

public class RealmManager {
    private static readonly Logger _log = new(typeof(RealmManager));

    public static ImmutableDictionary<int, World> Worlds = ImmutableDictionary.Create<int, World>();
    public static ImmutableDictionary<int, User> Users = ImmutableDictionary.Create<int, User>();
    public static ImmutableDictionary<int, Account> Accounts = ImmutableDictionary.Create<int, Account>();
    public static ImmutableList<string> ActiveRealms = ImmutableList.Create<string>();

    private static int _nextWorldId;
    
    public static void Init() {
        AddWorld(new Nexus());
        
        _log.Info("Realm Manager initialized.");
    }

    public static void AddWorld(World world) {
        world.Id = world.Id == 0 ? GetNextWorldId() : world.Id;
        Worlds = Worlds.Add(world.Id, world);
    }

    public static void UserConnected(User user) {
        Users = Users.Add(user.Id, user);
        SendServerProjectiles(user);
        user.StartNetwork();
        _log.Debug($"User {user.Id} connected from {user.Network.IP}");
    }
    
    public static void UserDisconnected(User user) {
        Users = Users.Remove(user.Id);
    }

    public static int GetNextWorldId() {
        return Interlocked.Increment(ref _nextWorldId);
    }
    
    public static string GetNewRealmName() {
        string ret = null;
        while (ret == null || ActiveRealms.Any(i => i.EqualsIgnoreCase(ret)))
            ret = RealmConfig.Config.Names.RandomElement();
        return ret;
    }

    public static async Task<bool> ReloadAllBehaviors() {
        var success = BehaviorLibrary.Reload(GameServerConfig.Config.BehaviorsDir);
        if (!success)
            return false;

        await Task.Run(() => {
            foreach (var world in Worlds) {
                foreach (var behavior in world.Value.EntityBehaviors) {
                    behavior.Load();
                }
            }
        });
        return true;
    }

    public static void BroadcastAll(Action<User> act) {
        foreach (var user in Users.Values)
            act(user);
    }
    
    private static void SendServerProjectiles(User user) { // TODO: Send all projectiles in 1 packet instead of per-projectile -_-
        foreach (var type in Shoot.CustomProjectileOwners) {
            var desc = XmlLibrary.ObjectDescs[type];
            foreach (var conProps in desc.Projectiles.Custom) {
                var props = conProps.Props;
                user.SendPacket(new ServerProjectileProps(
                    type, conProps.ProjectileIndex, props.ObjectId, props.LifetimeMS, props.MultiHit, props.PassesCover,
                    props.ArmorPiercing, props.Size, props.Effects)
                );
            }
        }
    }
}
