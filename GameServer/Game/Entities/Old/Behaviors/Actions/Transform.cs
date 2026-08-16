using Common.Resources.Xml;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Transform : BehaviorScript {
    private readonly string _target;

    public Transform(string target) {
        _target = target;
    }

    public override void Start(ref EntityView host) {
        var obj = XmlLibrary.Id2Object(_target);
        if (obj.Class.Contains("Portal"))
            return;

        var hostId = host.Id;
        var spawnX = host.Stats.Pos.X;
        var spawnY = host.Stats.Pos.Y;
        var isSpawned = host.Stats.Flags.IsSet((int)EntityFlags.Spawned);
        var world = host.World;
        GameLogic.Enqueue(() => {
            var entity = new Entity(obj.ObjectType);
            ref var newEn = ref world.EnterWorld(ref entity);
            ref var enStats = ref world.EntityStats.Get(newEn.Id);
            if (isSpawned)
                enStats.Flags.Set((int)EntityFlags.Spawned);

            enStats.Move(spawnX, spawnY);

            world.LeaveWorld(hostId);
        });
    }
}