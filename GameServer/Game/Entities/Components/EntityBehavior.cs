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

public struct EntityBehavior : IIdentifiable, IDisposable {
    private static readonly Logger _log = new(typeof(EntityBehavior));

    public int Id { get; set; }

    public readonly World World;
    public readonly HashSet<State> ActiveStates = [];
    public readonly HashSet<BehaviorTransition> PastTransitions = [];
    public readonly StateResourceController Resources = new();
    
    public int ParentId;

    private readonly string _objectId;

    private State _rootState;
    private State _currentState;

    public EntityBehavior(World world, ref Entity en) {
        Id = en.Id;
        World = world;
        _objectId = XmlLibrary.ObjectDescs[en.ObjectType].ObjectId;
    }

    public void Load(State rootState) {
        _rootState = rootState;
        Resources.ClearResources();

        _currentState = rootState.GetDeepState();
        var view = new EntityView(World, Id);
        _currentState.Enter(ref view);
    }

    public void TransitionTo(string targetState, ref RealmTime time) {
        if (_currentState == null)
            return;

        var view = new EntityView(World, Id);
        _currentState.Exit(ref view, ref time);

        if (_rootState.States.TryGetValue(targetState, out var newState)) {
            _currentState.ExitInactiveParent(ref view, time,
                newState); // Calls parent's Exit method if it's not parent of the new State

            _currentState = newState.GetDeepState();
            _currentState.Enter(ref view);
        }
        else {
            _log.Error($"{_objectId}: State {targetState} not found.");
            _currentState = null;
        }
    }

    public void Tick(ref RealmTime time) {
        if (_currentState == null)
            return;

        var view = new EntityView(World, Id);
        var targetState = _currentState.Tick(ref view, ref time);
        if (targetState != null)
            TransitionTo(targetState, ref time);
    }

    public void Dispose() {
        ActiveStates.Clear();
        PastTransitions.Clear();
        Resources.ClearResources();
        _currentState = null;
    }
}