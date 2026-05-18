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
        host.World.Enqueue(w => {
            var entity = new Entity(obj.ObjectType);
            ref var newEn = ref w.EnterWorld(ref entity);
            ref var enStats = ref w.EntityStats.Get(newEn.Id);
            if (isSpawned)
                enStats.Flags.Set((int)EntityFlags.Spawned);

            enStats.Move(spawnX, spawnY);

            w.LeaveWorld(hostId);
        });
    }
}