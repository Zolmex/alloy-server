using Common.Game;
using GameServer.Game.Entities;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class EntityNotWithinTransition : BehaviorTransition {
    private readonly float radius;
    private readonly string target;
    private readonly BehaviorScript.TargetType targetType;

    public EntityNotWithinTransition(string targetState, string target = "player", float radius = 8f,
        TransitionType transitionType = TransitionType.Random)
        : base(transitionType) {
        RegisterTargetStates(targetState);
        this.target = target;
        this.radius = radius;
        targetType = this.target == "player" ? BehaviorScript.TargetType.Player : BehaviorScript.TargetType.Entity;
    }

    public EntityNotWithinTransition(string target = "player", float radius = 8f,
        TransitionType transitionType = TransitionType.Random, params string[] targetStates)
        : base(transitionType) {
        RegisterTargetStates(targetStates);
        this.target = target;
        this.radius = radius;
        targetType = this.target == "player" ? BehaviorScript.TargetType.Player : BehaviorScript.TargetType.Entity;
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
        if (targetType == BehaviorScript.TargetType.Player && !host.World.Map.GetPlayersWithin(host.Stats.Pos, radius).Any()) return GetTargetState();

        if (targetType == BehaviorScript.TargetType.Entity && !host.World.Map.GetEntitiesByName(host.Stats.Pos, target, radius).Any())
            return GetTargetState();

        return null;
    }
}