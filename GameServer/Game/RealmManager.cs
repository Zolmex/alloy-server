using System.Collections.Immutable;
using System.Diagnostics;
using Common.Database.Models;
using Common.Resources.Xml;
using Common.Utilities;
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
    
    public static void Init() {
        AddWorld(new Nexus());
        
        _log.Info("Realm Manager initialized.");
    }

    public static void AddWorld(World world) {
        Worlds = Worlds.Add(world.Id, world);
    }

    public static void UserConnected(User user) {
        Users = Users.Add(user.Id, user);
        SendServerProjectiles(user);
        user.StartNetwork();
        _log.Debug($"User {user.Id} connected from {user.Network.IP}");
    }
    
    private static void SendServerProjectiles(User user) {
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
    
    public static void UserDisconnected(User user) {
        Users = Users.Remove(user.Id);
    }
}