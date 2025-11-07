#region

using Common.Database;
using Common.Resources.Config;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.IO;
using System.Text;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.HELLO)]
public class Hello : IIncomingPacket
{
    public string BuildVersion;
    public int GameId;
    public string Username;
    public string Password;
    public int MapJSONLength;
    public string MapJSON;

    public void Read(NetworkReader rdr)
    {
        BuildVersion = rdr.ReadUTF();
        GameId = rdr.ReadInt32();
        Username = rdr.ReadUTF();
        Password = rdr.ReadUTF();
        MapJSONLength = rdr.ReadInt32();
        MapJSON = Encoding.UTF8.GetString(rdr.ReadBytes(MapJSONLength));
    }

    public async void Handle(User user)
    {
        if (BuildVersion != GameServerConfig.Config.Version)
        {
            user.SendFailure(Failure.INCORRECT_VERSION, GameServerConfig.Config.Version);
            return;
        }

        var acc = user.Account;
        if (user.State != ConnectionState.Reconnecting)
        {
            var verify = await DbClient.VerifyAccount(Username, Password);
            var status = verify.Item2;
            acc = verify.Item1;
            if (acc == null)
            {
                user.SendFailure(Failure.DEFAULT, status.GetDescription());
                return;
            }
        }

        if (acc == null)
        {
            user.SendFailure(Failure.DEFAULT, "Invalid user state.");
            return;
        }

        if (RealmManager.UserAccIds.TryGetValue(user, out _) && user.State != ConnectionState.Reconnecting)
        {
            user.SendFailure(Failure.ACCOUNT_IN_USE, $"Account in use: {Username}/{acc.AccountId}");
            return;
        }

        if (GameServerConfig.Config.AdminOnly && !acc.Admin)
        {
            user.SendFailure(Failure.DEFAULT, "Admin only server.");
            return;
        }

        RealmManager.Worlds.TryGetValue(GameId, out var world);

        if (GameId == World.TEST_ID)
        {
            if (!acc.Admin)
            {
                user.SendFailure(Failure.FORCE_CLOSE_GAME, "Only players with admin permissions can make test maps.");
                return;
            }

            if (MapJSONLength < 1 || string.IsNullOrEmpty(MapJSON))
            {
                user.SendFailure(Failure.FORCE_CLOSE_GAME, "Invalid test map data.");
                return;
            }

            var mapFolder = $"{GameServerConfig.Config.WorldsDir}/testMaps"; // Save map in directory

            if (!Directory.Exists(mapFolder))
                Directory.CreateDirectory(mapFolder);

            File.WriteAllText($"{mapFolder}/{acc.Name}_{DateTime.Now.Ticks}.jm", MapJSON);

            world = new TestWorld("Test");
            world.DisplayName = $"{acc.Name}'s Test World";
            world.LoadJsonMap(MapJSON, world.DisplayName);
            await RealmManager.AddWorld(world);
        }
        else if (world == null)
        {
            user.SendFailure(Failure.DEFAULT, $"Invalid target world: {GameId}");
            return;
        }

        var seed = (uint)new Random().Next(1, int.MaxValue);
        user.SetGameInfo(acc, seed, world);

        user.SendPacket(new MapInfo(
            world.Map.Width,
            world.Map.Height,
            world.Config.Name,
            world.DisplayName,
            seed,
            world.Config.Background,
            world.Config.ShowDisplays,
            world.Config.AllowTeleport,
            world.Music,
            world.Config.Difficulty));
    }

    public override string ToString()
    {
        var type = typeof(Hello);
        var props = type.GetProperties();
        var ret = $"\n";
        foreach (var prop in props)
        {
            ret += $"{prop.Name}:{prop.GetValue(this)}";
            if (!(props.IndexOf(prop) == props.Length - 1))
                ret += "\n";
        }

        return ret;
    }
}