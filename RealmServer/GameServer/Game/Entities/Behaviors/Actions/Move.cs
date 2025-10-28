#region

using Common;
using Common.Utilities;
using GameServer.Utilities;
using System;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class MoveInfo
    {
        public int CooldownMs;
        public long MoveStarted;
        public Vector2 StartPos;
        public bool Moving;
    }

    public record Move : BehaviorScript
    {
        private readonly Vector2 _move;
        private readonly float _moveTime;
        private readonly int _moveTimeMs;
        private readonly int _cooldownMsDefault;
        private readonly Ease _ease;

        public Move(float xDist = 0f, float yDist = 0f, int cooldownMs = 1000, float tilesPerSecond = 1f,
            Ease ease = Ease.None)
        {
            _move = new Vector2(xDist, yDist);
            _moveTime = MathF.Sqrt((xDist * xDist) + (yDist * yDist)) / tilesPerSecond;
            _moveTimeMs = (int)(_moveTime * 1000);
            _cooldownMsDefault = cooldownMs;
            _ease = ease;
        }

        public Move(XElement xml)
        {
            var xDist = xml.GetAttribute<float>("xDist", 0f);
            var yDist = xml.GetAttribute<float>("yDist", 0f);
            var tilesPerSecond = xml.GetAttribute<float>("tilesPerSecond", 1f);
            _move = new Vector2(xDist, yDist);
            _moveTime = MathF.Sqrt((xDist * xDist) + (yDist * yDist)) / tilesPerSecond;
            _moveTimeMs = (int)(_moveTime * 1000);
            _cooldownMsDefault = xml.GetAttribute<int>("cooldownMS", 1000);
            _ease = Enum.Parse<Ease>(xml.GetAttribute<string>("ease", "None"));
        }

        public override void Start(Character host)
        {
            var moveInfo = host.ResolveResource<MoveInfo>(this);
            moveInfo.CooldownMs = _cooldownMsDefault;
            moveInfo.Moving = false;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var moveInfo = host.ResolveResource<MoveInfo>(this);
            var firstMove = false;
            if (!moveInfo.Moving && moveInfo.CooldownMs > 0)
            {
                moveInfo.CooldownMs -= time.ElapsedMsDelta;
                if (moveInfo.CooldownMs > 0)
                    return BehaviorTickState.OnCooldown;
            }
            else if (!moveInfo.Moving && moveInfo.CooldownMs <= 0)
            {
                moveInfo.CooldownMs = _moveTimeMs - time.ElapsedMsDelta;
                moveInfo.Moving = true;
                moveInfo.StartPos = host.Position.ToVec2();
                moveInfo.MoveStarted = time.TotalElapsedMs;
                firstMove = true;
            }

            var elapsedTimePerc = (time.TotalElapsedMs - moveInfo.MoveStarted) / 1000f / _moveTime;
            if (_ease != Ease.None)
                Easing.EaseVal(_ease, ref elapsedTimePerc);
            host.Move(moveInfo.StartPos + (_move * elapsedTimePerc));
            if (firstMove)
                return BehaviorTickState.BehaviorActivate;

            moveInfo.CooldownMs -= time.ElapsedMsDelta;
            if (moveInfo.CooldownMs <= 0)
            {
                moveInfo.Moving = false;
                moveInfo.CooldownMs = _cooldownMsDefault;
                return BehaviorTickState.BehaviorDeactivate;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}