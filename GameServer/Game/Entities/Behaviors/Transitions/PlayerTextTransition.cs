using System.Text.RegularExpressions;
using Common.Game;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Components;
using GameServer.Game.Entities.Old.Events;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class PlayerTextInfo {
    public Regex Rgx;
    public bool Transition;
}

public class PlayerTextTransition : BehaviorTransition {
    private readonly bool _ignoreCase;
    private readonly string _regex;

    public PlayerTextTransition(string targetState, string regex, bool ignoreCase = true) {
        RegisterTargetStates(targetState);
        _regex = regex;
        _ignoreCase = ignoreCase;
    }

    public override void Start(BehaviorController controller) {
        var state = host.Behavior.Resources.ResolveResource<PlayerTextInfo>(this);
        state.Rgx = _ignoreCase ? new Regex(_regex, RegexOptions.IgnoreCase) : new Regex(_regex);
    }

    public override string Tick(BehaviorController controller, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<PlayerTextInfo>(this);
        foreach (var text in host.World.TextCache) {
            var match = state.Rgx.Match(text);
            if (!match.Success)
                continue;

            state.Transition = true;
        }

        return state.Transition ? GetTargetState() : null;
    }
}