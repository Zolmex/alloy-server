using System.Text.RegularExpressions;
using Common.Game;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;

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

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<PlayerTextInfo>(this);
        state.Rgx = _ignoreCase ? new Regex(_regex, RegexOptions.IgnoreCase) : new Regex(_regex);
    }

    public override string Tick(ref EntityView host, ref RealmTime time) {
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