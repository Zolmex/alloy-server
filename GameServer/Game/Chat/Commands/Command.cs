using System;
using GameServer.Game.Entities;
using GameServer.Game.Network;

namespace GameServer.Game.Chat.Commands;

public enum CommandPermissionLevel {
    Player = 0,
    Donor1 = 10,
    Donor2 = 20,
    Donor3 = 30,
    Donor4 = 40,
    Creative = 50,
    Moderator = 80,
    Developer = 90,
    Admin = 100
}

public class Command {
    public CommandPermissionLevel PermissionLevel;
    public virtual void Execute(User user, string args) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute {
    public CommandAttribute(string command, CommandPermissionLevel permissionLevel, string[] aliases = null) {
        Command = command;
        PermissionLevel = permissionLevel;
        Aliases = aliases;
    }

    public string Command { get; }

    public string[] Aliases { get; }

    public CommandPermissionLevel PermissionLevel { get; }
}