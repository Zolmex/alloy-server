#region

using Common.Utilities;
using GameServer.Game.Entities.Loot;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Behaviors.Classic
{
    public class State : IStateChild
    {
        private static readonly Logger _log = new(typeof(State));

        public readonly Dictionary<string, State> States = new();

        public string Name { get; }
        public State Parent { get; private set; }
        public List<BehaviorTransition> Transitions { get; }
        public List<BehaviorScript> Scripts { get; }
        public List<State> ChildStates { get; }
        public CharacterLoot Loot { get; }

        public State(string name, params IStateChild[] children)
            : this(children)
        {
            Name = name;
        }

        public State(CharacterLoot loot, params IStateChild[] children)
            : this(children)
        {
            Loot = loot;
        }

        public State(params IStateChild[] children)
        {
            Transitions = new List<BehaviorTransition>(); // Need to register resources for these
            Scripts = new List<BehaviorScript>();
            ChildStates = new List<State>();

            if (children == null)
                return;

            foreach (var child in children)
            {
                if (child is BehaviorTransition trans)
                    Transitions.Add(trans);
                else if (child is BehaviorScript script)
                    Scripts.Add(script);
                else if (child is State state)
                {
                    state.AddToDictionary(States);
                    state.Parent = this;
                    ChildStates.Add(state);
                }
            }
        }

        public void RegisterLoot(Character host)
        {
            if (Loot == null)
            {
                return;
            }

            foreach (var loot in Loot.Loots)
            {
                host.RegisterLoot(loot);
            }
        }

        public void AddToDictionary(Dictionary<string, State> dict)
        {
            dict.Add(Name, this);
            foreach (var state in ChildStates)
                state.AddToDictionary(dict);
        }

        public State GetDeepState()
        {
            if (ChildStates.Count != 0)
                return ChildStates[0].GetDeepState();
            return this;
        }

        public void Enter(Character host) // Perform any initial setups we need for current and child states
        {
            if (!host.ClassicBehavior.ActiveStates.Add(this))
                return;

            Parent?.Enter(host); // Call parent to enter

            foreach (var trans in Transitions)
            {
                trans.Start(host);
            }

            foreach (var script in Scripts)
            {
                script.Start(host);
            }
        }

        public string Tick(Character host, RealmTime time)
        {
            var targetState = Parent?.Tick(host, time);
            if (targetState != null)
                return targetState;

            foreach (var trans in Transitions)
            { // Check if we have a transition to make
                targetState = trans.Tick(host, time);
                if (targetState != null && host.ClassicBehavior.PastTransitions.Add(trans))
                { // Make sure transitions only occur once (prevents parent state transitions to happen multiple times)
                    return targetState;
                }
            }

            foreach (var script in Scripts)
                script.Tick(host, time);

            return null;
        }

        public void Exit(Character host, RealmTime time)
        {
            if (!host.ClassicBehavior.ActiveStates.Remove(this)) // Prevents exiting the same state twice
                return;

            // Instead of clearing resources (we might end up deleting parent state's resources), remove each script's resources
            foreach (var script in Scripts)
            {
                script.End(host, time);
                host.StateResources.RemoveResource(script);
            }

            foreach (var trans in Transitions)
            {
                host.ClassicBehavior.PastTransitions.Remove(trans);
                trans.End(host, time);
            }
        }

        public void ExitInactiveParent(Character host, RealmTime time, State targetState)
        { // Will exit parent if it is not parent of the targetState
            if (Parent == null)
                return;

            if (Parent.ChildStates.Contains(targetState))
                return;

            Parent.Exit(host, time);
            Parent.ExitInactiveParent(host, time, targetState); // Recursively call exit on each parent
        }
    }
}