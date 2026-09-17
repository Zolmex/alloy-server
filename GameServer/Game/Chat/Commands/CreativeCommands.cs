using Common.Resources.Xml;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network;

namespace GameServer.Game.Chat.Commands;

[Command("spawn", CommandPermissionLevel.Player)]
public class SpawnCommand : Command {
    public override async Task ExecuteAsync(User user, string args) {
        // if (user.GameInfo.Account.Rank < (int)CommandPermissionLevel.Moderator && player.World is not TestWorld) {
        //     user.SendError("Can only use this command in a test world.");
        //     return;
        // }

        if (string.IsNullOrWhiteSpace(args)) {
            user.SendHelp("/spawn <count> <entity>");
            return;
        }

        var rgs = args.Split(' ');

        int spawnCount;
        if (!int.TryParse(rgs[0], out spawnCount))
            spawnCount = -1;

        var desc = XmlLibrary.Id2Object(string.Join(' ', spawnCount == -1 ? rgs : rgs.Skip(1)), false);
        if (spawnCount == -1)
            spawnCount = 1;

        if (desc == null) {
            user.SendError("null object desc");
            return;
        }

        if (desc.Player) {
            user.SendError("Can't spawn this entity");
            return;
        }

        user.SendInfo($"Spawning <{spawnCount}> <{desc.DisplayId}> in 2 seconds");

        var world = user.Session.World;
        var pos = world.Ecs.Get<Position>(user.Session.Player).Pos;
        var x = pos.X;
        var y = pos.Y;

        world.AddTimedAction(2000, w => {
            for (var i = 0; i < spawnCount; i++) {
                var en = w.EnterWorld(desc);
                ref var enPos = ref w.Ecs.Get<Position>(en);
                enPos.Move(x, y);
            }
        });
    }
}