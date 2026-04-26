#region

using System;
using System.Numerics;
using Common.Network;
using Common.Resources.Xml.Descriptors;

#endregion

namespace Common.Projectiles.ProjectilePaths;

public class ChangeSpeedPath : ProjectilePathSegment {
    public int Cooldown;
    public int CooldownOffset;

    public float Increment;
    public int Repeat;

    public ChangeSpeedPath(float speed, float increment, int cooldown, float? angle = null, int? lifetimeMs = null,
        int cooldownOffset = 0, int repeat = 999999, int? timeOffset = null, params PathSegmentModifier[] mods)
        : base(PathType.ChangeSpeedPath, speed, angle, lifetimeMs, timeOffset, mods) {
        Increment = increment;
        Cooldown = cooldown;
        CooldownOffset = cooldownOffset;
        Repeat = repeat;
    }

    public override Vector2 PositionAt(int elapsedLifetimeMs) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
            return p;

        elapsedLifetimeMs -= TimeOffset;

        ApplyModifiers(ref elapsedLifetimeMs);

        var dist = Math.Clamp(elapsedLifetimeMs, 0, CooldownOffset) * (Speed / 1000f); // 0 -> cooldown offset

        if (elapsedLifetimeMs > CooldownOffset) // cooldown offset -> end
        {
            elapsedLifetimeMs -= CooldownOffset;
            var increments = Math.Min(elapsedLifetimeMs / Cooldown, Repeat);
            for (var i = 1; i <= increments; i++)
                dist += Cooldown * (Speed + i * Increment) / 1000f;
            var relElapsed = elapsedLifetimeMs - Cooldown * increments;
            dist += relElapsed * (Speed + (increments + 1) * Increment) / 1000f;
        }

        p.X = dist * MathF.Cos(Angle);
        p.Y = dist * MathF.Sin(Angle);
        return p;
    }

    public override void Write(ref SpanWriter wtr) {
        base.Write(ref wtr);
        wtr.Write(Increment);
        wtr.Write(Cooldown);
        wtr.Write(CooldownOffset);
        wtr.Write(Repeat);
    }

    public override ProjectilePathSegment Clone() {
        return new ChangeSpeedPath(Speed, Increment, Cooldown, _angle, _lifetimeMs, CooldownOffset, Repeat, TimeOffset);
    }
}