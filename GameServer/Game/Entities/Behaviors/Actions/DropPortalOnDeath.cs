using System;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Entities.Events;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Worlds;

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

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(HandleDeath);
    }

    private void HandleDeath(ref DeathEvent evt) {
        var host = new EntityView(evt.World, evt.HostId);

        host.Events.OnDeath.Unsubscribe(HandleDeath);
        if (host.World.DisplayName.Contains("Arena") || host.Stats.Flags.IsSet((int)EntityFlags.Spawned))
            return;

        if (Random.Shared.NextDouble() <= _probability) {
            var portalDesc = XmlLibrary.Id2Object(_portalId);
            var timeoutTime = _timeout == null ? portalDesc.XML.GetValue<int>("Timeout") : _timeout;

            var entity = new Entity(portalDesc.ObjectType);
            ref var en = ref host.World.EnterWorld(ref entity);
            var childX = host.Stats.Pos.X + (float)Random.Shared.NextDouble() * 1.5f;
            var childY = host.Stats.Pos.Y + (float)Random.Shared.NextDouble() * 1.5f;
            en.Move(host.World, childX, childY);

            var id = en.Id;
            if (timeoutTime != 0)
                host.World.AddTimedAction(timeoutTime * 1000, w => w.LeaveWorld(id));
        }
    }
}