#region

using GameServer.Game.Entities;
using System;
using static GameServer.Game.Chat.Commands.Command;

#endregion

namespace GameServer.Game.Chat.Commands;

public class Command
{
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

    public CommandPermissionLevel PermissionLevel;
    public virtual void Execute(Player player, string args) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string command, CommandPermissionLevel permissionLevel, string[] aliases = null)
    {
        Command = command;
        PermissionLevel = permissionLevel;
        Aliases = aliases;
    }

    public string Command { get; }

    public string[] Aliases { get; }

    public CommandPermissionLevel PermissionLevel { get; }
}