#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using System;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class LinePath : ProjectilePathSegment
    {
        public LinePath(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
            : base(PathType.LinePath, speed, angle, lifetimeMs, timeOffset, mods)
        { }

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;
            
            elapsedLifetimeMs -= TimeOffset;
            
            ApplyModifiers(ref elapsedLifetimeMs);
            
            var dist = elapsedLifetimeMs * (Speed / 1000f);
            p.X = dist * MathF.Cos(Angle);
            p.Y = dist * MathF.Sin(Angle);
            return p;
        }

        public override ProjectilePathSegment Clone()
        {
            return new LinePath(Speed, _angle, _lifetimeMs, TimeOffset);
        }
    }
}