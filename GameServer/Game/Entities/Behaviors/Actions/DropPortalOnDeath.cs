using System;
using Arch.Core;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record DropPortalOnDeath : BehaviorScript {
    private readonly string _portalId;
    private readonly float _probability;
    private readonly int _timeout;

    public DropPortalOnDeath(string portalId, float probability = 1, int timeout = 0) {
        _portalId = portalId;
        _probability = probability;
        _timeout = timeout;
    }

    public override void Start(ref EntityContext host) {
        host.World.EventSystem.Subscribe(host.Entity, HandleDeath);
    }

    private void HandleDeath(ref DeathEvent evt) {
        var host = new EntityContext(evt.World, evt.Entity);

        if (host.World.DisplayName.Contains("Arena") || host.Flags.Mask.IsSet((int)EntityFlags.Spawned))
            return;

        if (Random.Shared.NextDouble() <= _probability) {
            var portalDesc = XmlLibrary.Id2Object(_portalId);
            var timeoutTime = _timeout == null ? portalDesc.XML.GetValue<int>("Timeout") : _timeout;

            var en = host.World.EnterWorld(portalDesc);
            ref var enPos = ref host.World.Ecs.Get<Position>(en);
            var childX = host.Position.Pos.X + (float)Random.Shared.NextDouble() * 1.5f;
            var childY = host.Position.Pos.Y + (float)Random.Shared.NextDouble() * 1.5f;
            enPos.Move(childX, childY);

            if (timeoutTime != 0)
                host.World.AddTimedAction(timeoutTime * 1000, w => w.LeaveWorld(en));
        }
    }
}