using GameServer.Game.Entities.Extensions;
using GameServer.Game.Network;

namespace GameServer.Game.Chat.Commands;

[Command("commands", CommandPermissionLevel.Player)]
public class CommandListCommand : Command {
    public override void Execute(User user, string args) {
        var cmdList = string.Join(", ", CommandManager.GetCommandList(user.GameInfo.Account.Rank));
        user.SendInfo($"Available commands: {cmdList}");
    }
}