using Common;
using Common.Game;

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

    public override string Tick(ref EntityView host, ref RealmTime time) {
        var entityId = host.World.Map.GetNearestEntityByName(_entity, host.Stats.Pos.X, host.Stats.Pos.Y, _dist);
        if (entityId == 0)
            return null;

        ref var enStats = ref host.World.EntityStats.Get(entityId);
        var hpPerc = (float)enStats.GetInt(StatType.HP) / enStats.GetInt(StatType.MaxHP);
        var transition = hpPerc <= _threshold;
        return transition ? GetTargetState() : null;
    }
}