using System.Text.RegularExpressions;
using GameServer.Game.Entities.Types;

namespace GameServer.Game.Entities.Behaviors.Transitions;

public class PlayerTextInfo
{
    public Regex Rgx;
    public bool Transition;
}

public class PlayerTextTransition : BehaviorTransition
{
    private readonly bool _ignoreCase;
    private readonly string _regex;

    public PlayerTextTransition(string targetState, string regex, bool ignoreCase = true)
    {
        RegisterTargetStates(targetState);
        _regex = regex;
        _ignoreCase = ignoreCase;
    }

    public override void Start(CharacterEntity host)
    {
        var state = host.ResolveResource<PlayerTextInfo>(this);
        state.Rgx = _ignoreCase ? new Regex(_regex, RegexOptions.IgnoreCase) : new Regex(_regex);
        host.OnPlayerText += HandlePlayerText;
    }

    public override void End(CharacterEntity host, RealmTime time)
    {
        host.OnPlayerText -= HandlePlayerText;
    }

    public override string Tick(CharacterEntity host, RealmTime time)
    {
        var state = host.ResolveResource<PlayerTextInfo>(this);
        return state.Transition ? GetTargetState() : null;
    }

    public void HandlePlayerText(CharacterEntity host, Player player, string text)
    {
        var state = host.ResolveResource<PlayerTextInfo>(this);
        var match = state.Rgx.Match(text);
        if (!match.Success)
            return;

        state.Transition = true;
    }
}