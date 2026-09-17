using System.Numerics;
using Common.Game;
using Common.Structs;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Components;

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

    public override void Start(ref EntityContext host) {
        var moveToState = host.BehavController.Resources.ResolveResource<MoveToState>(this);
        moveToState.StartPos = host.Position.Pos;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var moveToState = host.BehavController.Resources.ResolveResource<MoveToState>(this);
        var pos = _relative ? moveToState.StartPos + _targetPos : _targetPos;
        host.Position.MoveTowards(ref time, pos, _tilesPerSecond);
        return BehaviorTickState.BehaviorActive;
    }
}