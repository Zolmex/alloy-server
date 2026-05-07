using System;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityBehavior : IEntityComponent {
    private static readonly Logger _log = new(typeof(EntityBehavior));

    public int Id { get; set; }

    public readonly HashSet<State> ActiveStates = [];
    public readonly HashSet<BehaviorTransition> PastTransitions = [];
    public readonly StateResourceController Resources = new();

    private readonly World _world;
    private readonly string _objectId;

    private State _rootState;
    private State _currentState;

    public EntityBehavior(World world, ref Entity en) {
        _world = world;
        _objectId = XmlLibrary.ObjectDescs[en.ObjectType].ObjectId;
    }

    public void Load(State rootState) {
        _rootState = rootState;
        Resources.ClearResources();

        _currentState = rootState.GetDeepState();
        _currentState.Enter(ref this);
    }

    public void TransitionTo(string targetState, RealmTime time) {
        if (_currentState == null)
            return;

        _currentState.Exit(ref this, time);

        if (_rootState.States.TryGetValue(targetState, out var newState)) {
            _currentState.ExitInactiveParent(ref this, time,
                newState); // Calls parent's Exit method if it's not parent of the new State

            _currentState = newState.GetDeepState();
            _currentState.Enter(ref this);
        }
        else {
            _log.Error($"{_objectId}: State {targetState} not found.");
            _currentState = null;
        }
    }

    public void Tick(ref RealmTime time) {
        if (_currentState == null)
            return;

        var targetState = _currentState.Tick(ref this, time);
        if (targetState != null)
            TransitionTo(targetState, time);
    }

    public void Dispose() {
        ActiveStates.Clear();
        PastTransitions.Clear();
        Resources.ClearResources();
        _currentState = null;
    }
}