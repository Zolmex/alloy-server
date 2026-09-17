using Arch.Core;
using Common.Game;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Game.Entities.Components;

public record struct Behavior(Entity Parent) { }

public class BehaviorController : IDisposable {
    
    private static readonly Logger _log = new Logger(typeof(BehaviorController));
    
    public readonly Entity Host;
    public readonly World World;
    public readonly HashSet<State> ActiveStates = [];
    public readonly HashSet<BehaviorTransition> PastTransitions = [];
    public readonly StateResourceController Resources = new();

    private readonly string _objectId;
    private State _rootState;
    private State _currentState;

    public BehaviorController(World world, Entity host, string objectId) {
        Host = host;
        World = world;
        _objectId = objectId;
    }
    
    public void Load() {
        if (!BehaviorLibrary.ClassicBehaviors.TryGetValue(_objectId, out var rootState)) {
            _log.Error($"Behavior not found for '{_objectId}'");
            return;
        }
        
        _rootState = rootState;
        Resources.ClearResources();

        _currentState = rootState.GetDeepState();
        var host = new EntityContext(World, Host);
        _currentState.Enter(ref host);
    }

    public void TransitionTo(string targetState, ref RealmTime time) {
        if (_currentState == null)
            return;

        var host = new EntityContext(World, Host);
        _currentState.Exit(ref host, ref time);

        if (_rootState.States.TryGetValue(targetState, out var newState)) {
            _currentState.ExitInactiveParent(ref host, time,
                newState); // Calls parent's Exit method if it's not parent of the new State

            _currentState = newState.GetDeepState();
            _currentState.Enter(ref host);
        }
        else {
            _log.Error($"{_objectId}: State {targetState} not found.");
            _currentState = null;
        }
    }

    public void Tick(ref RealmTime time) {
        if (_currentState == null)
            return;

        var host = new EntityContext(World, Host);
        var targetState = _currentState.Tick(ref host, ref time);
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