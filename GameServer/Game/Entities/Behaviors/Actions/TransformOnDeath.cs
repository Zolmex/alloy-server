using System;
using Common.Game;
using Common.Resources.Xml;
using GameServer.Game.Entities.Components;
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

    public override void Start(ref EntityContext host) {
        host.World.EventSystem.Subscribe(host.Entity, HandleDeath);
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

        var host = new EntityContext(evt.World, evt.Entity);
        var isSpawned = host.Flags.Mask.IsSet((int)EntityFlags.Spawned);
        var count = Random.Shared.Next(_min, max + 1);
        for (var i = 0; i < count; i++) {
            var newEn = evt.World.EnterWorld(obj);
            ref var enPos = ref evt.World.Ecs.Get<Position>(newEn);
            enPos.Move(host.Position.Pos.X, host.Position.Pos.Y);
            if (isSpawned) {
                ref var newFlags = ref evt.World.Ecs.Get<Flags>(newEn);
                newFlags.Mask.Set((int)EntityFlags.Spawned);
            }
        }
    }
}