using System.Text.RegularExpressions;

namespace GameServer.Game.Entities.Behaviors.Transitions
{
    public class PlayerTextInfo
    {
        public bool Transition;
        public Regex Rgx;
    }
    
    public class PlayerTextTransition : BehaviorTransition
    {
        private readonly string _regex;
        private readonly bool _ignoreCase;
        
        public PlayerTextTransition(string targetState, string regex, bool ignoreCase = true)
        {
            RegisterTargetStates(targetState);
            _regex = regex;
            _ignoreCase = ignoreCase;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<PlayerTextInfo>(this);
            state.Rgx = _ignoreCase ? new Regex(_regex, RegexOptions.IgnoreCase) : new Regex(_regex);
            host.OnPlayerText += HandlePlayerText;
        }

        public override void End(Character host, RealmTime time)
        {
            host.OnPlayerText -= HandlePlayerText;
        }

        public override string Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<PlayerTextInfo>(this);
            return state.Transition ? GetTargetState() : null;
        }

        public void HandlePlayerText(Character host, Player player, string text)
        {
            var state = host.ResolveResource<PlayerTextInfo>(this);
            var match = state.Rgx.Match(text);
            if (!match.Success)
                return;

            state.Transition = true;
        }
    }
}