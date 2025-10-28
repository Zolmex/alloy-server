#region

using Common.Control;
using Common.Control.Message;
using Common.Utilities;
using GameServer.Game.Entities;
using System;

#endregion

namespace GameServer.Game.Chat.Commands
{
    [Command("dbrestore", CommandPermissionLevel.Admin)]
    public class DbRestoreCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            var spaceIndex = args.IndexOf(' ');

            if (spaceIndex == -1)
            {
                player.SendError($"Invalid arguments.");
                player.SendError($"Usage: /dbrestore [name/before] <backup name/backup date>");
                return;
            }

            var command = args[..spaceIndex];
            var argument = args[(spaceIndex + 1)..];

            var message = new RestoreCommandInfo();
            message.ShutdownDelay = TimeSpan.FromSeconds(5);

            switch (command)
            {
                case "name":
                    message.Name = argument;
                    ServerControl.Publish(ControlChannel.DbRestore, ServerControl.Host.InstanceID, null, message);
                    player.SendInfo($"Sent a restore by name command to DatabaseManager.");
                    return;

                case "before":
                    try
                    {
                        message.TimeAgo = TimeUtils.ParseTimeSpan(argument);
                        message.DateTime = TimeUtils.ParseDateTime(argument);
                    }
                    catch (Exception e)
                    {
                        player.SendError($"Invalid DateTime or TimeSpan format.");
                        return;
                    }

                    ServerControl.Publish(ControlChannel.DbRestore, ServerControl.Host.InstanceID, null, message);
                    player.SendInfo($"Sent a restore before date/time command to DatabaseManager.");
                    return;

                default:
                    player.SendError($"Invalid dbrestore command.");
                    player.SendError($"Usage: /dbrestore [name/before] <backup name/backup date>");
                    return;
            }
        }
    }

    [Command("dbbackup", CommandPermissionLevel.Admin)]
    public class DbBackupCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            ServerControl.Publish(ControlChannel.DbBackup, ServerControl.Host.InstanceID, null, args);
            player.SendInfo($"Sent a backup command to DatabaseManager.");
        }
    }

    [Command("dbwipe", CommandPermissionLevel.Admin)]
    public class DbWipeCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            RealmManager.BroadcastAllUsers(user => user.Disconnect());
            ServerControl.Publish(ControlChannel.DbWipe, ServerControl.Host.InstanceID, null, false);
            player.SendInfo($"Sent a wipe command to DatabaseManager.");
        }
    }
}