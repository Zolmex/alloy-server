#region

using System.Linq;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game.Entities.Behaviors.Transitions;

public class EntitiesNotWithinTransition : BehaviorTransition {
    private readonly float _radius;
    private readonly string[] _targets;

    public EntitiesNotWithinTransition(float radius, string targetStates, params string[] targets) {
        RegisterTargetStates(targetStates);
        _targets = targets;
        _radius = radius;
    }

    public override string Tick(CharacterEntity host, RealmTime time) {
        if (_targets == null) {
            if (!host.GetEnemiesWithin(_radius).Any())
                return GetTargetState();
            return null;
        }

        if (!host.GetOtherEnemiesByName(_targets, _radius).Any()) return GetTargetState();

        return null;
    }
}