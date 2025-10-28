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
    public class WanderInfo
    {
        public float AngleDir;
        public Vector2 InitialPos;
        public Vector2 StartPos;
        public long WanderStarted;
        public int WanderCooldown;
        public bool Wandering;
    }

    public record Wander : BehaviorScript
    {
        private readonly float _distance;
        private readonly float _moveTime;
        private readonly int _moveTimeMs;
        private readonly float _distanceFromSpawn;
        private readonly int _cooldownMs;
        private readonly Ease _ease;

        public Wander(float speed = 1f, float distance = 1f, float distanceFromSpawn = 5f, int cooldownMs = 0, Ease ease = Ease.None)
        {
            _distance = distance;
            _moveTime = distance / speed;
            _moveTimeMs = (int)(_moveTime * 1000);
            _distanceFromSpawn = distanceFromSpawn;
            _cooldownMs = cooldownMs;
            _ease = ease;
        }

        public Wander(XElement xml)
        {
            _distance = xml.GetAttribute<float>("distance", 4f);
            var speed = xml.GetAttribute<float>("speed", 1f);
            _moveTime = _distance / speed;
            _moveTimeMs = (int)(_moveTime * 1000);
            _distanceFromSpawn = xml.GetAttribute<float>("distanceFromSpawn", 5f);
            _cooldownMs = xml.GetAttribute<int>("cooldownMS", 0);
            _ease = Enum.Parse<Ease>(xml.GetAttribute<string>("ease", "None"));
        }

        public override void Start(Character host)
        {
            var wanderInfo = host.ResolveResource<WanderInfo>(this);
            wanderInfo.WanderCooldown = _cooldownMs;
            wanderInfo.InitialPos = new Vector2(host.Position.X, host.Position.Y);
            wanderInfo.Wandering = false;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var wanderInfo = host.ResolveResource<WanderInfo>(this);
            var firstMove = false;
            if (!wanderInfo.Wandering && wanderInfo.WanderCooldown > 0)
            {
                wanderInfo.WanderCooldown -= time.ElapsedMsDelta;
                if (wanderInfo.WanderCooldown > 0)
                    return BehaviorTickState.OnCooldown;
            }
            else if (!wanderInfo.Wandering && wanderInfo.WanderCooldown <= 0)
            {
                var intersect = Utils.FindCircleCircleIntersections(wanderInfo.InitialPos, _distanceFromSpawn, host.Position.ToVec2(), _distance, out var interSect1, out var intersect2);
                if (intersect == 2)
                {
                    var angle1Deg = host.GetAngleBetween(interSect1).Rad2Deg();
                    var angle2Deg = host.GetAngleBetween(intersect2).Rad2Deg();
                    if (angle2Deg < 0) angle2Deg += 360;
                    while (angle1Deg < angle2Deg)
                        angle1Deg += 360;
                    float angleDeg = new Random().Next((int)angle2Deg, (int)angle1Deg);
                    wanderInfo.AngleDir = angleDeg.Deg2Rad();
                }
                else if (intersect == 0)
                {
                    float angleDeg = new Random().Next(360);
                    wanderInfo.AngleDir = angleDeg.Deg2Rad();
                }

                wanderInfo.WanderCooldown = _moveTimeMs - time.ElapsedMsDelta;
                wanderInfo.Wandering = true;
                wanderInfo.WanderStarted = time.TotalElapsedMs;
                wanderInfo.StartPos = host.Position.ToVec2();
                firstMove = true;
            }

            var elapsedTimePerc = time.ElapsedMsDelta / 1000f / _moveTime;
            if (_ease != Ease.None)
                Easing.EaseVal(_ease, ref elapsedTimePerc);
            var relMove = new Vector2(MathF.Cos(wanderInfo.AngleDir) * elapsedTimePerc * _distance, MathF.Sin(wanderInfo.AngleDir) * elapsedTimePerc * _distance);
            host.MoveRelative(relMove);
            if (firstMove)
                return BehaviorTickState.BehaviorActivate;

            wanderInfo.WanderCooldown -= time.ElapsedMsDelta;
            if (wanderInfo.WanderCooldown <= 0)
            {
                wanderInfo.Wandering = false;
                wanderInfo.WanderCooldown = _cooldownMs;
                return BehaviorTickState.BehaviorDeactivate;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}