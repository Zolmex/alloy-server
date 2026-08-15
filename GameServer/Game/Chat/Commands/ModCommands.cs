using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Common;
using Common.Database;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Network;
using GameServer.Game.Worlds.Logic;

namespace GameServer.Game.Chat.Commands;

[Command("find", CommandPermissionLevel.Moderator)]
public class FindPlayerCommand : Command {
    public override async Task ExecuteAsync(User user, string args) {
        if (string.IsNullOrEmpty(args)) {
            user.SendError("Usage: /findPlayer {player}");
            return;
        }

        // Find locally first
        var target = RealmManager.Users.Values.FirstOrDefault(c => c.GameInfo.Account.Name == args)?.GameInfo.Data;
        if (target == null) {
            target = await Program.AccountServerRpc.GetUserInfo(args, -1);
            if (target == null) {
                user.SendError($"Player {args} not found.");
                return;
            }
        }

        user.SendInfo($"Player {args} is in {target?.WorldName}({target?.WorldId}) at {target?.Position}");
    }
}

// [Command("effAll", CommandPermissionLevel.Moderator)] // TODO: fix
// public class EffAllCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (!Enum.TryParse<ConditionEffectIndex>(args, out var eff)) {
//             user.SendError($"Invalid effect: {args}");
//             return;
//         }
//
//         player.World.BroadcastAll(plr => {
//             if (plr.HasConditionEffect(eff))
//                 plr.RemoveConditionEffect(eff);
//             else
//                 plr.ApplyConditionEffect(eff, -1);
//         });
//     }
// }

// [Command("kill", CommandPermissionLevel.Moderator)]
// public class KillCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         var entities = player.World.GetEnemiesByName(args, player.Position.X, player.Position.Y, 32f);
//         var entityCount = entities.Count();
//         foreach (var entity in entities)
//             entity.Death(player.Name);
//         var pluralCharacter = entityCount > 1 ? "s" : "";
//         if (entityCount > 0)
//             user.SendInfo($"You just killed {entityCount} {args}{pluralCharacter}. How could you?");
//     }
// }
//
// [Command("ban", CommandPermissionLevel.Moderator)]
// public class BanCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (string.IsNullOrEmpty(args)) {
//             user.SendError("Usage: /ban {player} \"{reason}\" {duration in days}");
//             return;
//         }
//
//         var reason = args.Split('\"')[1]; // Splits the args in 3, the 2nd index is the text between the " "
//         var words = args.Replace(reason, "")
//             .Split(' '); // Remove the reason because it might contain spaces, and find the other arguments
//         var targetName = words[0];
//         var durationInDays = int.Parse(words[2]);
//
//         var result = DbClient.BanAccountAsync(targetName, reason, DateTime.Now + TimeSpan.FromDays(durationInDays),
//             player.AccountId).Result;
//         var success = result.Success;
//         var error = result.Error;
//         if (success) {
//             user.SendInfo($"Player {targetName} successfully banned.");
//             RealmManager.TryDisconnectUserByName(targetName);
//         }
//         else {
//             user.SendError(error);
//         }
//     }
// }
//
// [Command("unban", CommandPermissionLevel.Moderator)]
// public class UnbanCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (string.IsNullOrEmpty(args)) {
//             user.SendError("Usage: /unban {player}");
//             return;
//         }
//
//         var unban = DbClient.UnbanAccountAsync(args).Result;
//         var success = unban.Success;
//         var error = unban.Error;
//         if (success) {
//             user.SendInfo($"Player {args} successfully unbanned.");
//             RealmManager.TryDisconnectUserByName(args);
//         }
//         else {
//             user.SendError(error);
//         }
//     }
// }

[Command("kick", CommandPermissionLevel.Moderator)]
public class KickCommand : Command {
    public override async Task ExecuteAsync(User user, string args) {
        if (string.IsNullOrEmpty(args)) {
            user.SendError("Usage: /kick {player}");
            return;
        }

        var target = RealmManager.Users.Values.FirstOrDefault(c => c.GameInfo.Account.Name == args);
        if (target == null) {
            user.SendError($"Player {args} could not be found.");
            return;
        }
        
        target.Disconnect();
        user.SendInfo($"Player {args} successfully disconnected.");
    }
}

// [Command("mute", CommandPermissionLevel.Moderator)] // TODO: fix
// public class MuteCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (string.IsNullOrEmpty(args)) {
//             user.SendError("Usage: /mute {player}");
//             return;
//         }
//
//         var mute = DbClient.MuteAccountAsync(args).Result;
//         var success = mute.Item1;
//         var error = mute.Item2;
//         if (success)
//             user.SendInfo($"Player {args} successfully muted.");
//         else
//             user.SendError(error);
//     }
// }
//
// [Command("unmute", CommandPermissionLevel.Moderator)]
// public class UnmuteCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (string.IsNullOrEmpty(args)) {
//             user.SendError("Usage: /unmute {player}");
//             return;
//         }
//
//         var unmute = DbClient.UnmuteAccountAsync(args).Result;
//         var success = unmute.Item1;
//         var error = unmute.Item2;
//         if (success)
//             user.SendInfo($"Player {args} successfully unmuted.");
//         else
//             user.SendError(error);
//     }
// }

// [Command("summon", CommandPermissionLevel.Moderator)]
// public class SummonCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         if (string.IsNullOrEmpty(args)) {
//             user.SendHelp("Usage: /summon (player name or all)");
//             return;
//         }
//
//         var targets = new List<EntityId>();
//         foreach (ref var en in user.GameInfo.World.Entities) {
//             if (en.Type != EntityType.Player)
//                 continue;
//
//             ref var stats = ref user.GameInfo.World.EntityStats.Get(en.Id);
//             if (args == "all" || stats.GetString(StatType.Name).ToLower() == args.ToLower()) {
//                 targets.Add(en.Id);
//
//                 if (args != "all")
//                     break;
//             }
//         }
//
//         if (targets.Count == 0)
//             return;
//
//         foreach (var target in targets) {
//             ref var plrStats = ref user.GameInfo.World.EntityStats.Get(target);
//             plrStats.TeleportTo(user.GameInfo.Player.);
//         }
//     }
// }

// [Command("closerealm", CommandPermissionLevel.Moderator)]
// public class CloseRealmCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         var world = player.World;
//         if (world == null)
//             return;
//
//         var isRealm = world is Realm;
//         if (!isRealm) {
//             user.SendError("This command can only be executed in a realm!");
//             return;
//         }
//
//         var realm = world as Realm;
//         realm.CloseRealm();
//         user.SendInfo("Realm closed!");
//     }
// }
//
// [Command("godAll", CommandPermissionLevel.Moderator)]
// public class GodAllCommand : Command {
//     public override async Task ExecuteAsync(User user, string args) {
//         player.World.BroadcastAll(plr => {
//             if (plr.HasConditionEffect(ConditionEffectIndex.Invincible))
//                 plr.RemoveConditionEffect(ConditionEffectIndex.Invincible);
//             else
//                 plr.ApplyConditionEffect(ConditionEffectIndex.Invincible, -1);
//         });
//     }
// }

[Command("announce", CommandPermissionLevel.Moderator)]
public class AnnounceCommand : Command {
    public override async Task ExecuteAsync(User user, string args) {
        ChatManager.Announce(args);
    }
}