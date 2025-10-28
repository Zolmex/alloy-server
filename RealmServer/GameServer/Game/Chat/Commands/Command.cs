#region

using GameServer.Game.Entities;
using System;
using static GameServer.Game.Chat.Commands.Command;

#endregion

namespace GameServer.Game.Chat.Commands
{
    public class Command
    {
        public CommandPermissionLevel PermissionLevel;
        public virtual void Execute(Player player, string args) { }

        public enum CommandPermissionLevel
        {
            All = 0,
            Donor1 = 10,
            Donor2 = 20,
            Donor3 = 30,
            Donor4 = 40,
            Creative = 50,
            Moderator = 80,
            Developer = 90,
            Admin = 100
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        private string _command;
        public string Command => _command;

        private string[] _aliases;
        public string[] Aliases => _aliases;

        private CommandPermissionLevel _permission;
        public CommandPermissionLevel PermissionLevel => _permission;

        public CommandAttribute(string command, CommandPermissionLevel permissionLevel, string[] aliases = null)
        {
            _command = command;
            _permission = permissionLevel;
            _aliases = aliases;
        }
    }
}