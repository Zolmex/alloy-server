#region

using GameServer.Game.Entities;

#endregion

namespace GameServer.Game.Chat.Commands
{
    [Command("chunk", CommandPermissionLevel.Developer)]
    public class ChunkDataCommand : Command
    {
        public override void Execute(Player player, string args)
        {
            var chunk = player.Tile?.Chunk;
            if (chunk == null)
            {
                player.SendError("Null chunk");
                return;
            }

            player.SendInfo($"CX: {chunk.CX}; CY: {chunk.CY}; Players: {chunk.Players.Count}; Entities: {chunk.Entities.Count}");
        }
    }
}