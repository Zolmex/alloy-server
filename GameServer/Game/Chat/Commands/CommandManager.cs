using System.Reflection;
using Common.Utilities;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Network;

namespace GameServer.Game.Chat.Commands;

public static class CommandManager {
    private static readonly Dictionary<string, Command> _commands = new();
    private static Logger _log;

    public static void Load() {
        _log = new Logger(typeof(CommandManager));

        var types = Assembly.GetExecutingAssembly().GetTypes();
        for (var i = 0; i < types.Length; i++) {
            var type = types[i];
            if (!type.IsAbstract && type.IsSubclassOf(typeof(Command))) {
                var cmd = (Command)Activator.CreateInstance(type);
                var commandAttribute = Attribute.GetCustomAttribute(type, typeof(CommandAttribute)) as CommandAttribute;
                cmd.PermissionLevel = commandAttribute.PermissionLevel;
                _commands.Add($"/{commandAttribute.Command.ToLower()}", cmd);

                if (commandAttribute.Aliases is null)
                    continue;

                foreach (var alias in commandAttribute.Aliases)
                    if (!_commands.TryAdd($"/{alias.ToLower()}", cmd))
                        _log.Error($"Conflict: alias '{alias}' already exists.");
            }
        }
    }

    public static void ExecuteCommand(User user, string name, string args) {
        if (!_commands.TryGetValue(name.ToLower(), out var cmd)) {
            user.SendError($"Command not found: {name}");
            return;
        }

        if (cmd.PermissionLevel == CommandPermissionLevel.Admin && (!user.GameInfo.Account.IsAdmin ||
                                                                            (int)cmd.PermissionLevel >
                                                                            user.GameInfo.Account.Rank)) {
            user.SendError("You're not authorized to use this command.");
            return;
        }

        cmd.Execute(user, args);
    }

    public static IEnumerable<string> GetCommandList(int accRank) {
        foreach (var kvp in _commands) {
            if (kvp.Value.PermissionLevel > (CommandPermissionLevel)accRank)
                continue;
            yield return kvp.Key;
        }
    }
}