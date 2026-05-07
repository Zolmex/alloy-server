using System.Collections.Generic;
using System.Linq;
using Common.Game;
using Common.Utilities;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors;

public class BehaviorTransition(TransitionType transitionType = TransitionType.Random) : IStateChild {
    private readonly List<string> targetStates = new();
    private readonly List<string> usedStates = new();

    private TransitionType TransitionType => transitionType;

    public void RegisterTargetStates(params string[] targetStates) {
        foreach (var targetState in targetStates) this.targetStates.Add(targetState);
    }

    public string GetTargetState() {
        if (usedStates.Count == targetStates.Count) usedStates.Clear();

        switch (TransitionType) {
            case TransitionType.Random:
                return targetStates.RandomElement();
            case TransitionType.Random7Bag:
                var targetState = targetStates.Except(usedStates).RandomElement();
                usedStates.Add(targetState);
                return targetState;
            case TransitionType.Sequential:
                targetState = targetStates[usedStates.Count];
                usedStates.Add(targetState);
                return targetState;
        }

        return null; // never happens but compiler is loud.
    }

    public virtual void Start(ref EntityView host) { }

    public virtual string Tick(ref EntityView host, ref RealmTime time) {
        return null;
    }

    public virtual void End(ref EntityView host, ref RealmTime time) { }
}