using System.Linq;
using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class EntitiesNotWithinTransition : BehaviorTransition {
    private readonly float _radius;
    private readonly string[] _targets;

    public EntitiesNotWithinTransition(float radius, string targetStates, params string[] targets) {
        RegisterTargetStates(targetStates);
        _targets = targets;
        _radius = radius;
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        if (_targets == null) {
            foreach (var _ in host.World.Map.GetEntitiesWithin(host.Stats.Pos, _radius))
                return null;
            return GetTargetState();
        }

        if (!host.World.Map.GetEntitiesByName(host.Stats.Pos, _targets, _radius).Any())
            return GetTargetState();

        return null;
    }
}