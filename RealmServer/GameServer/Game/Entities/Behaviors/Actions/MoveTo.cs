#region

using Common;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class MoveToState
    {
        public Vector2 StartPos;
    }

    public record MoveTo : BehaviorScript
    {
        private readonly Vector2 _targetPos;
        private readonly float _tilesPerSecond;
        private readonly bool _relative;

        public MoveTo(float x = 0, float y = 0, float tilesPerSecond = 1f, bool relative = false)
        {
            _targetPos = new Vector2(x, y);
            _tilesPerSecond = tilesPerSecond;
            _relative = relative;
        }

        public override void Start(Character host)
        {
            var moveToState = host.ResolveResource<MoveToState>(this);
            moveToState.StartPos = host.Position.ToVec2();
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var moveToState = host.ResolveResource<MoveToState>(this);
            var pos = _relative ? moveToState.StartPos + _targetPos : _targetPos;
            host.MoveTowards(time, pos, _tilesPerSecond);
            return BehaviorTickState.BehaviorActive;
        }
    }
}