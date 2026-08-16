using System;
using Common.Game;
using Common.Resources.Xml;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record TransformOnDeath : BehaviorScript {
    private readonly int _max;
    private readonly int _min;
    private readonly float _probability;
    private readonly string _target;

    public TransformOnDeath(string target, int min = 1, int max = 1, float probability = 1) {
        _target = target;
        _min = min;
        _max = max;
        _probability = probability;
    }

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(HandleDeath);
    }

    private void HandleDeath(ref DeathEvent evt) {
        if (!(Random.Shared.NextDouble() <= _probability))
            return;

        var obj = XmlLibrary.Id2Object(_target);
        if (obj.Class.Contains("Portal"))
            return;

        var max = _max;
        if (_min > _max)
            max = _min;

        var count = Random.Shared.Next(_min, max + 1);
        for (var i = 0; i < count; i++) {
            var entity = new Entity(obj.ObjectType);
            evt.World.EnterWorld(ref entity);
            ref var hostStats = ref evt.World.EntityStats.Get(evt.HostId);
            ref var enStats = ref evt.World.EntityStats.Get(entity.Id);
            if (hostStats.Flags.IsSet((int)EntityFlags.Spawned))
                enStats.Flags.Set((int)EntityFlags.Spawned);

            enStats.Move(hostStats.Pos.X, hostStats.Pos.Y);
        }
    }
}