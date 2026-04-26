#region

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Database;
using Common.Utilities;
using GameServer.Game.Entities.Types;
using GameServer.Game.Worlds.Logic;

#endregion

namespace GameServer.Game.Chat.Commands;

[Command("findPlayer", CommandPermissionLevel.Moderator)]
public class FindPlayerCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /findPlayer {player}");
            return;
        }

        var target = player.World.GetPlayerByName(args);
        if (target == null) {
            target = RealmManager.Users.Values.FirstOrDefault(u => u.Account.Name.EqualsIgnoreCase(args))?.GameInfo
                .Player;
            if (target == null) {
                player.SendError($"Player {args} not found.");
                return;
            }
        }

        player.SendInfo($"Player {args} is in {target.World.Name}({target.World.Id}) at {target.Position}");
    }
}

[Command("effAll", CommandPermissionLevel.Moderator)]
public class EffAllCommand : Command {
    public override void Execute(Player player, string args) {
        if (!Enum.TryParse<ConditionEffectIndex>(args, out var eff)) {
            player.SendError($"Invalid effect: {args}");
            return;
        }

        player.World.BroadcastAll(plr => {
            if (plr.HasConditionEffect(eff))
                plr.RemoveConditionEffect(eff);
            else
                plr.ApplyConditionEffect(eff, -1);
        });
    }
}

[Command("kill", CommandPermissionLevel.Moderator)]
public class KillCommand : Command {
    public override void Execute(Player player, string args) {
        var entities = player.World.GetEnemiesByName(args, player.Position.X, player.Position.Y, 32f);
        var entityCount = entities.Count();
        foreach (var entity in entities)
            entity.Death(player.Name);
        var pluralCharacter = entityCount > 1 ? "s" : "";
        if (entityCount > 0)
            player.SendInfo($"You just killed {entityCount} {args}{pluralCharacter}. How could you?");
    }
}

[Command("ban", CommandPermissionLevel.Moderator)]
public class BanCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /ban {player} \"{reason}\" {duration in days}");
            return;
        }

        var reason = args.Split('\"')[1]; // Splits the args in 3, the 2nd index is the text between the " "
        var words = args.Replace(reason, "")
            .Split(' '); // Remove the reason because it might contain spaces, and find the other arguments
        var targetName = words[0];
        var durationInDays = int.Parse(words[2]);

        var result = DbClient.BanAccountAsync(targetName, reason, DateTime.Now + TimeSpan.FromDays(durationInDays),
            player.AccountId).Result;
        var success = result.Success;
        var error = result.Error;
        if (success) {
            player.SendInfo($"Player {targetName} successfully banned.");
            RealmManager.TryDisconnectUserByName(targetName);
        }
        else {
            player.SendError(error);
        }
    }
}

[Command("unban", CommandPermissionLevel.Moderator)]
public class UnbanCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /unban {player}");
            return;
        }

        var unban = DbClient.UnbanAccountAsync(args).Result;
        var success = unban.Success;
        var error = unban.Error;
        if (success) {
            player.SendInfo($"Player {args} successfully unbanned.");
            RealmManager.TryDisconnectUserByName(args);
        }
        else {
            player.SendError(error);
        }
    }
}

[Command("kick", CommandPermissionLevel.Moderator)]
public class KickCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /kick {player}");
            return;
        }

        if (RealmManager.TryDisconnectUserByName(args))
            player.SendInfo($"Player {args} successfully disconnected.");
        else
            player.SendError($"Player {args} could not be found.");
    }
}

[Command("mute", CommandPermissionLevel.Moderator)]
public class MuteCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /mute {player}");
            return;
        }

        var mute = DbClient.MuteAccountAsync(args).Result;
        var success = mute.Item1;
        var error = mute.Item2;
        if (success)
            player.SendInfo($"Player {args} successfully muted.");
        else
            player.SendError(error);
    }
}

[Command("unmute", CommandPermissionLevel.Moderator)]
public class UnmuteCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendError("Usage: /unmute {player}");
            return;
        }

        var unmute = DbClient.UnmuteAccountAsync(args).Result;
        var success = unmute.Item1;
        var error = unmute.Item2;
        if (success)
            player.SendInfo($"Player {args} successfully unmuted.");
        else
            player.SendError(error);
    }
}

[Command("summon", CommandPermissionLevel.Moderator)]
public class SummonCommand : Command {
    public override void Execute(Player player, string args) {
        if (string.IsNullOrEmpty(args)) {
            player.SendHelp("Usage: /summon (player name or all)");
            return;
        }

        var targets = new List<Player>();
        foreach (var plr in player.World.Players.Values)
            if (args == "all" || plr.Name.ToLower() == args.ToLower()) {
                targets.Add(plr);

                if (args != "all")
                    break;
            }

        if (targets.Count == 0)
            return;

        foreach (var target in targets)
            target.TeleportTo(player.Position, true);
    }
}

[Command("closerealm", CommandPermissionLevel.Moderator)]
public class CloseRealmCommand : Command {
    public override void Execute(Player player, string args) {
        var world = player.World;
        if (world == null)
            return;

        var isRealm = world is Realm;
        if (!isRealm) {
            player.SendError("This command can only be executed in a realm!");
            return;
        }

        var realm = world as Realm;
        realm.CloseRealm();
        player.SendInfo("Realm closed!");
    }
}

[Command("godAll", CommandPermissionLevel.Moderator)]
public class GodAllCommand : Command {
    public override void Execute(Player player, string args) {
        player.World.BroadcastAll(plr => {
            if (plr.HasConditionEffect(ConditionEffectIndex.Invincible))
                plr.RemoveConditionEffect(ConditionEffectIndex.Invincible);
            else
                plr.ApplyConditionEffect(ConditionEffectIndex.Invincible, -1);
        });
    }
}

[Command("announce", CommandPermissionLevel.Moderator)]
public class AnnounceCommand : Command {
    public override void Execute(Player player, string args) {
        ChatManager.Announce(args);
    }
}