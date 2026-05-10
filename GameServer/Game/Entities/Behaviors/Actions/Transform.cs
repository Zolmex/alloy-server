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

        var entity = new Entity(obj.ObjectType);
        host.World.EnterWorld(ref entity);
        ref var enStats = ref host.World.EntityStats.Get(entity.Id);
        if (host.Stats.Flags.IsSet((int)EntityFlags.Spawned))
            enStats.Flags.Set((int)EntityFlags.Spawned);

        enStats.Move(host.Stats.Pos.X, host.Stats.Pos.Y);

        host.World.LeaveWorld(host.Id);
    }
}