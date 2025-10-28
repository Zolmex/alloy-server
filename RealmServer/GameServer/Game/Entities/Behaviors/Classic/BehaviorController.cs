#region

using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Classic;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class BehaviorController
    {
        private static readonly Logger _log = new(typeof(BehaviorController));

        public State RootState { get; }
        public HashSet<State> ActiveStates { get; } // List of states that are currently active (includes root state)
        public HashSet<BehaviorTransition> PastTransitions { get; } = []; // List of states the parent state has transitioned to, child state transitions are cleared on exit

        private readonly Character _host;
        private State _currentState;

        public BehaviorController(Character host, State rootState)
        {
            _host = host;
            ActiveStates = new HashSet<State>();

            RootState = rootState;
            RootState.RegisterLoot(host);
        }

        public void Initialize()
        {
            _currentState = RootState.GetDeepState();
            _currentState.Enter(_host);
        }

        public void TransitionTo(string targetState, RealmTime time)
        {
            if (_currentState == null)
                return;

            _currentState.Exit(_host, time);

            if (RootState.States.TryGetValue(targetState, out var newState))
            {
                _currentState.ExitInactiveParent(_host, time, newState); // Calls parent's Exit method if it's not parent of the new State

                _currentState = newState.GetDeepState();
                _currentState.Enter(_host);
            }
            else
            {
                _log.Error($"{_host.Name}: State {targetState} not found.");
                _currentState = null;
            }
        }

        public void Tick(RealmTime time)
        {
            if (_currentState == null)
                return;

            var targetState = _currentState.Tick(_host, time); // If this returns something that means a transition has occured
            if (targetState != null)
                TransitionTo(targetState, time);
        }
    }
}