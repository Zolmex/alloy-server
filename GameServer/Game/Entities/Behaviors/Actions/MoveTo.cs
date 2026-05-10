using System.Numerics;
using Common.Game;
using Common.Structs;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class MoveToState {
    public WorldPosData StartPos;
}

public record MoveTo : BehaviorScript {
    private readonly bool _relative;
    private readonly WorldPosData _targetPos;
    private readonly float _tilesPerSecond;

    public MoveTo(float x = 0, float y = 0, float tilesPerSecond = 1f, bool relative = false) {
        _targetPos = new WorldPosData(x, y);
        _tilesPerSecond = tilesPerSecond;
        _relative = relative;
    }

    public override void Start(ref EntityView host) {
        var moveToState = host.Behavior.Resources.ResolveResource<MoveToState>(this);
        moveToState.StartPos = host.Stats.Pos;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var moveToState = host.Behavior.Resources.ResolveResource<MoveToState>(this);
        var pos = _relative ? moveToState.StartPos + _targetPos : _targetPos;
        host.Stats.MoveTowards(ref time, ref pos, _tilesPerSecond);
        return BehaviorTickState.BehaviorActive;
    }
}