using GameServer.Game.Entities;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record RemoveObjectOnDeath : BehaviorScript {
    private readonly string _objName;
    private readonly int _range;

    public RemoveObjectOnDeath(string objName, int range) {
        _objName = objName;
        _range = range;
    }

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(OnDeath);
    }

    public void OnDeath(ref DeathEvent evt) {
        ref var stats = ref evt.World.EntityStats.Get(evt.HostId);
        foreach (ref var en in evt.World.Map.GetEntitiesWithin(stats.Pos, _range))
            if (en.Desc.ObjectId == _objName)
                evt.World.LeaveWorld(en.Id);
    }
}