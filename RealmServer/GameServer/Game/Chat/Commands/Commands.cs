#region

using Common;
using Common.Database;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;
using System;
using System.Linq;
using GameServer.Game.Entities.Types;
using GameServer.Game.Worlds.Logic;

#endregion

namespace GameServer.Game.Chat.Commands;

[Command("commands", CommandPermissionLevel.All)]
public class CommandListCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var cmdList = string.Join(", ", CommandManager.GetCommandList(player));
        player.SendInfo($"Available commands: {cmdList}");
    }
}

[Command("getQuest", CommandPermissionLevel.All)]
public class GetQuestCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var quest = player.Quest;
        if (quest == null)
        {
            player.SendError("You don't have a quest.");
            return;
        }

        player.SendInfo($"Quest is at X:{quest.Position.X} Y:{quest.Position.Y}");
    }
}

[Command("accId", CommandPermissionLevel.All)]
public class AccIdCommand : Command
{
    public override void Execute(Player player, string args)
    {
        player.SendInfo($"Account id: {player.AccountId}");
    }
}

[Command("trade", CommandPermissionLevel.All)]
public class TradeCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (string.IsNullOrEmpty(args))
        {
            if (player.PotentialPartner != null)
            {
                player.TradeRequest(player.PotentialPartner.Name);
                return;
            }

            player.SendError("No pending trades");
            return;
        }

        if (!player.World.PlayerNames.TryGetValue(args, out _))
        {
            player.SendError("Player " + args + " not found");
            return;
        }

        player.TradeRequest(args);
    }
}

[Command("reloadvault", CommandPermissionLevel.All)]
public class ReloadVaultCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var vault = player.User.GameInfo.Vault;
        if (vault == null || player.World == vault)
            return;

        vault.Delete();
    }
}

[Command("pos", CommandPermissionLevel.All)]
public class PosCommand : Command
{
    public override void Execute(Player player, string args)
    {
        player.SendInfo($"{player.Position.X} {player.Position.Y}");
    }
}

[Command("online", CommandPermissionLevel.All)]
public class OnlineCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var totalCount = RealmManager.Users.Count;
        var localCount = player.World.Players.Count;
        player.SendInfo($"There are {totalCount} players online. {localCount} of them are in this world.");
    }
}

[Command("realm", CommandPermissionLevel.All)]
public class RealmCommand : Command
{
    public override void Execute(Player player, string args)
    {
        if (!RealmManager.Worlds.TryGetValue(RealmManager.ActiveRealms.Keys.RandomElement(), out var realm))
        {
            player.SendError("No realms available");
            return;
        }

        player.User.ReconnectTo(realm);
    }
}

[Command("vault", CommandPermissionLevel.All)]
public class VaultCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var vault = player.User.GameInfo.Vault;
        if (vault == null)
        {
            vault = new Vault(player.User);
            RealmManager.AddWorld(vault);
        }
        else if (!vault.Active)
            RealmManager.AddWorld(vault);

        player.User.ReconnectTo(vault);
    }
}

[Command("uptime", CommandPermissionLevel.All)]
public class UptimeCommand : Command
{
    public override void Execute(Player player, string args)
    {
        var time = TimeSpan.FromMilliseconds(RealmManager.WorldTime.TotalElapsedMs);
        player.SendInfo(
            $"The server has been up for {time.Days} days, {time.Hours} hours and {time.Seconds} seconds.");
    }
}

[Command("glands", CommandPermissionLevel.All)]
public class GodlandsCommand : Command
{
    private static readonly WorldPosData[] GLANDS =
    [
        new(1512, 1048),
        new(983, 985),
        new(808, 992)
    ];

    public override void Execute(Player player, string args)
    {
        if (player.World is not Realm)
        {
            return;
        }

        var pos = GLANDS[player.World.MapId];
        if (!player.TeleportTo(pos))
            player.SendError($"Wait {player.TPCooldownLeft / 1000}");
    }
}