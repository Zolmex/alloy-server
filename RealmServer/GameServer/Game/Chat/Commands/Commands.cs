#region

using Common;
using Common.Database;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;
using System;
using System.Linq;

#endregion

namespace GameServer.Game.Chat.Commands
{
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

    [Command("pcreate", CommandPermissionLevel.All)]
    public class PartyCreateCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            player.CreateParty();
        }
    }

    [Command("pinvite", CommandPermissionLevel.All)]
    public class PartyInviteCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError($"You are not in a party.");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                player.SendError($"usage: /pinvite <name>.");
                return;
            }

            var toInvite = args.Split(' ');

            foreach (var targetName in toInvite)
            {
                var target = RealmManager.GetActivePlayers().FirstOrDefault(p => p.Name == targetName);

                if (target is null)
                {
                    player.SendInfo($"User {targetName} is not online or does not exist.");
                    continue;
                }

                target.InviteToParty(player);
            }
        }
    }

    [Command("paccept", CommandPermissionLevel.All)]
    public class PartyAcceptCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                player.SendError($"usage: /paccept <code>.");
                return;
            }

            var code = int.Parse(args);

            player.AcceptPartyInvite(code);
        }
    }

    [Command("pkick", CommandPermissionLevel.All)]
    public class PartyKickCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError($"You are not in a party.");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                player.SendError($"usage: /party kick <name>.");
                return;
            }

            var party = player.Party;

            var toKick = args.Split(' ');
            var activePlayers = player.Party.GetOnlinePlayers()
                .Where(p => toKick.Contains(p.Name))
                .ToDictionary(p => p.Name, p => p);

            foreach (var targetName in toKick)
            {
                if (activePlayers.TryGetValue(targetName, out var activePlayer))
                {
                    activePlayer.LeaveParty();

                    party?.BroadcastInfo($"{activePlayer.Name} was kicked from the party.");
                }
                else
                {
                    var acc = DbClient.GetAccountByName(targetName).Result;

                    if (acc is not null)
                    {
                        if (acc.PartyId != player.PartyId)
                            continue;

                        acc.PartyId = -1;
                        DbClient.Save(acc);

                        player.Party.RemoveMember(acc.AccountId);
                        party?.BroadcastInfo($"{acc.Name} was kicked from the party.");
                        continue;
                    }

                    player.SendInfo($"User {targetName} does not exist.");
                }
            }
        }
    }

    [Command("pleave", CommandPermissionLevel.All)]
    public class PartyLeaveCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            player.Party.BroadcastInfo($"{player.Name} left the party.");
            player.LeaveParty();
        }
    }

    [Command("pdisband", CommandPermissionLevel.All)]
    public class PartyDisbandCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            player.Party.Disband();
        }
    }

    [Command("psummon", CommandPermissionLevel.All)]
    public class PartySummonCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                player.Party.SummonAll(player);
                return;
            }

            var toSummon = args.Split(' ');

            var targetPlayers = RealmManager.GetActivePlayers().Where(p => toSummon.Contains(p.Name));
            foreach (var target in targetPlayers)
            {
                if (!player.Party.IsMember(target))
                {
                    player.SendError($"{target.Name} is not in your party.");
                    continue;
                }

                player.Party.SummonPlayer(target, player);
            }
        }
    }

    [Command("pjoin", CommandPermissionLevel.All)]
    public class PartyJoinCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            player.Party.JoinSummon(player);
        }
    }

    [Command("pdeny", CommandPermissionLevel.All)]
    public class PartyDenyCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            player.Party.DenySummon(player);
        }
    }

    [Command("plist", CommandPermissionLevel.All)]
    public class PartyListCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            var members = player.Party.MembersIds.Select(id => DbClient.GetAccount(id).Result);
            var players = player.Party.GetOnlinePlayers();

            var membersStr = string.Join(", ", members.Select(m => m.Name));
            var playersStr = string.Join(", ", players.Select(p => p.Name));
            player.SendInfo($"Members: {membersStr}");
            player.SendInfo($"Online: {playersStr}");
        }
    }

    [Command("phelp", CommandPermissionLevel.All)]
    public class PartyHelpCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            SendPartyHelp(player);
        }

        public static void SendPartyHelp(Player player)
        {
            player.SendInfo($"'/pcreate' to create a party.");
            player.SendInfo($"'/pinvite <name(s)>' to invite online player(s) to your party.");
            player.SendInfo($"'/paccept <code>' to accept a party invite.");
            player.SendInfo($"'/pkick <name(s)>' to kick party member(s) (only the party's creator is able to execute this command).");
            player.SendInfo($"'/pleave' to leave your current party.");
            player.SendInfo($"'/pdisband' to disband your current party (only the creator can).");
            player.SendInfo($"'/pjoin' to accept a summon invite.");
            player.SendInfo($"'/pdeny' to decline a summon invite.");
            player.SendInfo($"'/psummon' to summon the party to your current world.");
            player.SendInfo($"'/psummon <name(s)>' to summon specific party members to your world.");
            player.SendInfo($"'/p <message>' to talk in the party chat.");
        }
    }

    [Command("p", CommandPermissionLevel.All)]
    public class PartyChatCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                player.SendHelp("Usage: /p <message>");
                return;
            }

            if (!player.IsInParty)
            {
                player.SendError("You are not in a party.");
                return;
            }

            player.Party.BroadcastChat(args, player);
        }
    }
}