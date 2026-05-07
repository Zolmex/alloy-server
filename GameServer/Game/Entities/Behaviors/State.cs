using System.Collections.Generic;
using Common.Game;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors;

public class State : IStateChild {
    private static readonly Logger _log = new(typeof(State));

    public string Name { get; }
    public State Parent { get; private set; }
    public List<BehaviorTransition> Transitions { get; }
    public List<BehaviorScript> Scripts { get; }
    public List<State> ChildStates { get; }

    public readonly Dictionary<string, State> States = new();

    public State(string name, params IStateChild[] children)
        : this(children) {
        Name = name;
    }

    public State(params IStateChild[] children) {
        Transitions = new List<BehaviorTransition>(); // Need to register resources for these
        Scripts = new List<BehaviorScript>();
        ChildStates = new List<State>();

        if (children == null)
            return;

        foreach (var child in children)
            if (child is BehaviorTransition trans) {
                Transitions.Add(trans);
            }
            else if (child is BehaviorScript script) {
                Scripts.Add(script);
            }
            else if (child is State state) {
                state.AddToDictionary(States);
                state.Parent = this;
                ChildStates.Add(state);
            }
    }

    public void AddToDictionary(Dictionary<string, State> dict) {
        dict.Add(Name, this);
        foreach (var state in ChildStates)
            state.AddToDictionary(dict);
    }

    public State GetDeepState() {
        if (ChildStates.Count != 0)
            return ChildStates[0].GetDeepState();
        return this;
    }

    public void Setup(ObjectDesc desc) {
        foreach (var script in Scripts)
            script.Setup(desc);

        foreach (var child in ChildStates)
            child.Setup(desc);
    }

    public void Enter(ref EntityView host) // Perform any initial setups we need for current and child states
    {
        if (!host.Behavior.ActiveStates.Add(this))
            return;

        Parent?.Enter(ref host); // Call parent to enter

        foreach (var trans in Transitions) trans.Start(ref host);

        foreach (var script in Scripts) script.Start(ref host);
    }

    public string Tick(ref EntityView host, ref RealmTime time) {
        var targetState = Parent?.Tick(ref host, ref time);
        if (targetState != null)
            return targetState;

        foreach (var trans in Transitions) { // Check if we have a transition to make
            targetState = trans.Tick(ref host, ref time);
            if (targetState != null && host.Behavior.PastTransitions.Add(trans))
                // Make sure transitions only occur once (prevents parent state transitions to happen multiple times)
                return targetState;
        }

        foreach (var script in Scripts)
            script.Tick(ref host, ref time);

        return null;
    }

    public void Exit(ref EntityView host, ref RealmTime time) {
        if (!host.Behavior.ActiveStates.Remove(this)) // Prevents exiting the same state twice
            return;

        // Instead of clearing resources (we might end up deleting parent state's resources), remove each script's resources
        foreach (var script in Scripts) {
            script.End(ref host, ref time);
            host.Behavior.Resources.RemoveResource(script);
        }

        foreach (var trans in Transitions) {
            host.Behavior.PastTransitions.Remove(trans);
            trans.End(ref host, ref time);
        }
    }

    public void ExitInactiveParent(ref EntityView host, RealmTime time, State targetState) {
        // Will exit parent if it is not parent of the targetState
        if (Parent == null)
            return;

        if (Parent.ChildStates.Contains(targetState))
            return;

        Parent.Exit(ref host, ref time);
        Parent.ExitInactiveParent(ref host, time, targetState); // Recursively call exit on each parent
    }
}