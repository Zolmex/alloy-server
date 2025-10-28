#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Common.Utilities.Net;
using System;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class CirclePath : ProjectilePathSegment
    {
        public CirclePath(float rotationsPerSecond, float radius, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
            : base(PathType.CirclePath, rotationsPerSecond, angle, lifetimeMs, timeOffset, mods)
        {
            this.radius = radius;
        }

        private float radius;

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;
            
            elapsedLifetimeMs -= TimeOffset;
            
            ApplyModifiers(ref elapsedLifetimeMs);
            
            var elapsedSeconds = elapsedLifetimeMs / 1000f;
            float angle = 0;
            if (elapsedSeconds != 0)
                angle = Angle + (Speed * elapsedSeconds * 360f.Deg2Rad());

            p.X = MathF.Cos(angle) * radius;
            p.Y = MathF.Sin(angle) * radius;
            return p;
        }

        public override void Write(NetworkWriter wtr)
        {
            base.Write(wtr);
            wtr.Write(radius);
        }

        public override ProjectilePathSegment Clone()
        {
            return new CirclePath(Speed / 50, radius, _angle, _lifetimeMs, TimeOffset);
        }
    }
}