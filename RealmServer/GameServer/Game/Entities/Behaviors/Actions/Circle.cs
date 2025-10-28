#region

using Common.Utilities;
using System;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class CircleInfo
    {
        public float CurrentAngle;
    }

    public record Circle : BehaviorScript
    {
        private readonly float _rotationsPerSecond;
        private readonly float _acquireRadiusSqr;
        private readonly float _radius;
        private readonly string _target;

        public Circle(XElement xml)
        {
            _rotationsPerSecond = xml.GetAttribute<float>("rotationsPerSecond", 1f);
            _acquireRadiusSqr = (float)Math.Pow(xml.GetAttribute<float>("acquireRadius", 20f), 2);
            _radius = xml.GetAttribute<float>("radius", 1f);
            _target = xml.GetAttribute<string>("target", "player");
        }

        public Circle(float rotationsPerSecond = 1f, float acquireRadius = 20f, float radius = 4f, string target = "player")
        {
            _rotationsPerSecond = rotationsPerSecond;
            _acquireRadiusSqr = (float)Math.Pow(acquireRadius, 2);
            _radius = radius;
            _target = target;
        }

        public override void Start(Character host)
        {
            Character target;
            if (_target == "player")
                target = host.GetNearestPlayer(_acquireRadiusSqr);
            else
                target = host.World.GetNearestEnemyByName(_target, host.Position.X, host.Position.Y, _acquireRadiusSqr);

            if (target == null) return;

            var resource = host.ResolveResource<CircleInfo>(this);
            resource.CurrentAngle = host.GetAngleBetween(target).Rad2Deg();
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var resource = host.ResolveResource<CircleInfo>(this);
            var angleInc = 360f * (_rotationsPerSecond * time.ElapsedMsDelta / 1000);

            Character target;
            if (_target == "player")
                target = host.GetNearestPlayer(_acquireRadiusSqr);
            else
                target = host.World.GetNearestEnemyByName(_target, host.Position.X, host.Position.Y, _acquireRadiusSqr);


            if (target == null) return BehaviorTickState.BehaviorFailed;

            var targetPos = new Vector2(target.Position.X, target.Position.Y);
            targetPos += new Vector2(MathF.Cos(resource.CurrentAngle.Deg2Rad()), MathF.Sin(resource.CurrentAngle.Deg2Rad())) * _radius;
            if (host.Move(targetPos))
                resource.CurrentAngle += angleInc;
            return BehaviorTickState.BehaviorActive;
        }
    }
}