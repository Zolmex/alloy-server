#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using System;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class AcceleratePath : ProjectilePathSegment
    {
        public AcceleratePath(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
            : base(PathType.AcceleratePath, speed, angle, lifetimeMs, timeOffset, mods)
        { }

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var speed = Speed;
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;
            
            elapsedLifetimeMs -= TimeOffset;

            ApplyModifiers(ref elapsedLifetimeMs);

            speed *= elapsedLifetimeMs / (float)LifetimeMs;
            var dist = elapsedLifetimeMs * (speed / 1000f);

            p.X = dist * MathF.Cos(Angle);
            p.Y = dist * MathF.Sin(Angle);
            return p;
        }

        public override ProjectilePathSegment Clone()
        {
            return new AcceleratePath(Speed, _angle, _lifetimeMs, TimeOffset);
        }
    }
}