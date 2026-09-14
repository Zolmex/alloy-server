using System.Linq;
using Common.Game;
using GameServer.Game.Entities.Old;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class EntitiesWithinTransition : BehaviorTransition {
    private readonly float _radius;
    private readonly string[] _targets;

    public EntitiesWithinTransition(float radius, string targetStates, params string[] targets) {
        RegisterTargetStates(targetStates);
        _targets = targets;
        _radius = radius;
    }

    public override string Tick(BehaviorController controller, ref RealmTime time) {
        if (_targets == null) {
            foreach (var _ in host.World.Map.GetEntitiesWithin(host.Stats.Pos, _radius))
                return GetTargetState();
            return null;
        }

        if (host.World.Map.GetEntitiesByName(host.Stats.Pos, _targets, _radius).Any())
            return GetTargetState();

        return null;
    }
}