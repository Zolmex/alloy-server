#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using System;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class WavyPath : ProjectilePathSegment
    {
        public WavyPath(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
            : base(PathType.WavyPath, speed, angle, lifetimeMs, timeOffset, mods)
        { }

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;
            
            elapsedLifetimeMs -= TimeOffset;
            
            ApplyModifiers(ref elapsedLifetimeMs);
            
            var dist = elapsedLifetimeMs * (Speed / 1000f);
            var phase = Info.ProjId % 2 == 0 ? 0 : MathF.PI;
            var periodFactor = 6 * MathF.PI;
            var amplitudeFactor = MathF.PI / 64.0f;
            var theta = Angle + (amplitudeFactor * MathF.Sin(phase + (periodFactor * elapsedLifetimeMs / 1000.0f)));
            p.X = dist * MathF.Cos(theta);
            p.Y = dist * MathF.Sin(theta);
            return p;
        }

        public override ProjectilePathSegment Clone()
        {
            return new WavyPath(Speed, _angle, _lifetimeMs, TimeOffset);
        }
    }
}