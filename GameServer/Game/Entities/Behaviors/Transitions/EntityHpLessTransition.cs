using Arch.Core;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class EntityHpLessTransition : BehaviorTransition {
    private readonly float _dist;
    private readonly string _entity;
    private readonly float _threshold;

    public EntityHpLessTransition(float dist, string entity, float threshold, string targetState) {
        RegisterTargetStates(targetState);
        _threshold = threshold;
        _dist = dist;
        _entity = entity;
    }

    public override string Tick(ref EntityContext host, ref RealmTime time) {
        var entity = host.World.Map.GetNearestEntityByName(_entity, host.Position.Pos.X, host.Position.Pos.Y, _dist);
        if (entity == Entity.Null)
            return null;

        ref var enStats = ref host.World.Ecs.Get<Stats>(entity);
        var hpPerc = (float)enStats.GetInt(StatType.HP) / enStats.GetInt(StatType.MaxHP);
        var transition = hpPerc <= _threshold;
        return transition ? GetTargetState() : null;
    }
}