
using Common.Resources.Xml;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record RemoveObjectOnDeath : BehaviorScript {
    private readonly string _objName;
    private readonly int _range;

    public RemoveObjectOnDeath(string objName, int range) {
        _objName = objName;
        _range = range;
    }

    public override void Start(ref EntityContext host) {
        host.World.EventSystem.Subscribe(host.Entity, OnDeath);
    }

    public void OnDeath(ref DeathEvent evt) {
        ref var pos = ref evt.World.Ecs.Get<Position>(evt.Entity);
        foreach (var en in evt.World.Map.GetEntitiesWithin(pos.Pos, _range)) {
            var desc = XmlLibrary.ObjectDescs[evt.World.Ecs.Get<ObjectType>(en)];
            if (desc.ObjectId == _objName)
                evt.World.LeaveWorld(en);
        }
    }
}