#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using System;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class BoomerangPath : ProjectilePathSegment
    {
        public BoomerangPath(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
            : base(PathType.BoomerangPath, speed, angle, lifetimeMs, timeOffset, mods)
        { }

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;
            
            elapsedLifetimeMs -= TimeOffset;
            
            if (elapsedLifetimeMs > LifetimeMs / 2)
                elapsedLifetimeMs = LifetimeMs - elapsedLifetimeMs;
            var dist = elapsedLifetimeMs * (Speed / 1000f);
            p.X = dist * MathF.Cos(Angle);
            p.Y = dist * MathF.Sin(Angle);
            return p;
        }

        public override ProjectilePathSegment Clone()
        {
            return new BoomerangPath(Speed, _angle, _lifetimeMs, TimeOffset);
        }
    }
}