using Common.Resources.Xml;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Transform : BehaviorScript {
    private readonly string _target;

    public Transform(string target) {
        _target = target;
    }

    public override void Start(ref EntityContext host) {
        var obj = XmlLibrary.Id2Object(_target);
        if (obj.Class.Contains("Portal"))
            return;

        var hostId = host.Entity;
        var spawnX = host.Position.Pos.X;
        var spawnY = host.Position.Pos.Y;
        var isSpawned = host.Flags.Mask.IsSet((int)EntityFlags.Spawned);
        var world = host.World;
        GameLogic.Enqueue(() => {
            var newEn = world.EnterWorld(obj);
            ref var enStats = ref world.Ecs.Get<Position>(newEn);
            enStats.Move(spawnX, spawnY);
            if (isSpawned) {
                ref var enFlags = ref world.Ecs.Get<Flags>(newEn);
                enFlags.Mask.Set((int)EntityFlags.Spawned);
            }

            world.LeaveWorld(hostId);
        });
    }
}