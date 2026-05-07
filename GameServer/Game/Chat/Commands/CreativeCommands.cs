using Common.Resources.Xml;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Network;

namespace GameServer.Game.Chat.Commands;

[Command("spawn", CommandPermissionLevel.Player)]
public class SpawnCommand : Command {
    public override void Execute(User user, string args) {
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

        var world = user.GameInfo.World;
        ref var pos = ref world.EntityStats.Get(user.GameInfo.PlayerId).Pos;
        var x = pos.X;
        var y = pos.Y;

        var entity = new Entity(desc.ObjectType);
        world.AddTimedAction(2000, w => {
            for (var i = 0; i < spawnCount; i++) {
                ref var en = ref w.EnterWorld(ref entity);
                en.Move(world, x, y);
            }
        });
    }
}