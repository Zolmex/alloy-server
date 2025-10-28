#region

using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace GameServer.Game.Chat.Commands
{
    // This is basically the same implementation as RequestHandler in AppEngine
    public abstract class CommandManager
    {
        private static Dictionary<string, Command> _commands = new();
        protected static Logger _log;

        public CommandManager()
        { }

        public abstract void Execute(Player player, string args);

        public static IEnumerable<Player> GetPlayers(string args, Player player)
        {
            var playerList = new List<Player>() { player };
            if (player.AccRank < (int)Command.CommandPermissionLevel.Moderator)
                return playerList;

            switch (args)
            {
                case "self":
                    return playerList;
                case "nearby":
                    return player.World.GetAllPlayersWithin(player.Position.X, player.Position.Y, Player.SIGHT_RADIUS_SQR);
                case "all":
                    return player.World.Players.Values;
                case "server":
                    if (player.AccRank < (int)Command.CommandPermissionLevel.Admin)
                        return playerList;
                    return RealmManager.GetActivePlayers();
            }

            return playerList;
        }

        public static void Load()
        {
            _log = new Logger(typeof(CommandManager));

            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(Command)))
                {
                    var cmd = (Command)Activator.CreateInstance(type);
                    var commandAttribute = Attribute.GetCustomAttribute(type, typeof(CommandAttribute)) as CommandAttribute;
                    cmd.PermissionLevel = commandAttribute.PermissionLevel;
                    _commands.Add($"/{commandAttribute.Command.ToLower()}", cmd);

                    if (commandAttribute.Aliases is null)
                        continue;

                    foreach (var alias in commandAttribute.Aliases)
                    {
                        if (!_commands.TryAdd($"/{alias.ToLower()}", cmd))
                            _log.Error($"Conflict: alias '{alias}' already exists.");
                    }
                }
            }
        }

        public static void LogError(Exception e)
        {
            _log.Error(e);
        }

        public static void ExecuteCommand(Player player, string name, string args)
        {
            if (!_commands.TryGetValue(name.ToLower(), out var cmd))
            {
                player.SendError($"Command not found: {name}");
                return;
            }

            if (cmd.PermissionLevel == Command.CommandPermissionLevel.Admin && (!player.User.Account.Admin ||
                                                                                (int)cmd.PermissionLevel > player.User.Account.Rank))
            {
                player.SendError("You're not authorized to use this command.");
                return;
            }

            cmd.Execute(player, args);
        }

        public static IEnumerable<string> GetCommandList(Player player)
        {
            foreach (var kvp in _commands)
            {
                if (kvp.Value.PermissionLevel > (Command.CommandPermissionLevel)player.AccRank)
                    continue;
                yield return kvp.Key;
            }
        }
    }
}