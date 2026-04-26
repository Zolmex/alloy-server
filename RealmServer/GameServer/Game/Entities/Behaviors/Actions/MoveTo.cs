#region

using Common;
using System.Numerics;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public class MoveToState
{
    public Vector2 StartPos;
}

public record MoveTo : BehaviorScript
{
    private readonly bool _relative;
    private readonly Vector2 _targetPos;
    private readonly float _tilesPerSecond;

    public MoveTo(float x = 0, float y = 0, float tilesPerSecond = 1f, bool relative = false)
    {
        _targetPos = new Vector2(x, y);
        _tilesPerSecond = tilesPerSecond;
        _relative = relative;
    }

    public override void Start(CharacterEntity host)
    {
        var moveToState = host.ResolveResource<MoveToState>(this);
        moveToState.StartPos = host.Position.ToVec2();
    }

    public override BehaviorTickState Tick(CharacterEntity host, RealmTime time)
    {
        var moveToState = host.ResolveResource<MoveToState>(this);
        var pos = _relative ? moveToState.StartPos + _targetPos : _targetPos;
        host.MoveTowards(time, pos, _tilesPerSecond);
        return BehaviorTickState.BehaviorActive;
    }
}