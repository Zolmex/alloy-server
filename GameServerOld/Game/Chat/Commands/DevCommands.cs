#region

using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Chat.Commands;

[Command("chunk", CommandPermissionLevel.Developer)]
public class ChunkDataCommand : Command {
    public override void Execute(Player player, string args) {
        var chunk = player.Tile?.Chunk;
        if (chunk == null) {
            player.SendError("Null chunk");
            return;
        }

        player.SendInfo(
            $"CX: {chunk.CX}; CY: {chunk.CY}; Players: {chunk.Players.Count}; Entities: {chunk.Entities.Count}");
    }
}

[Command("reloadbehaviors", CommandPermissionLevel.Developer)]
public class ReloadBehaviorsCommand : Command {
    public override void Execute(Player player, string args) {
        player.SendInfo("Reloading behavior files...");
        if (RealmManager.ReloadAllBehaviors())
            player.SendInfo("Successfully reloaded behaviors.");
        else player.SendError("Failed to reload behaviors. Check GameServer console for more information.");
    }
}