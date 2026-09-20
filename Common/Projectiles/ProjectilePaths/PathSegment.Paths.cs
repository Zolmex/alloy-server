using System;
using System.Numerics;
using Common.Utilities;

namespace Common.Projectiles.ProjectilePaths;

public partial struct PathSegment {
    private readonly Vector2 PositionLine(int elapsed, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        var dist = elapsed * (Speed / 1000f);
        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));
        return p;
    }

    private readonly Vector2 PositionWavy(int elapsed, int projId, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        var dist = elapsed * (Speed / 1000f);
        var phase = projId % 2 == 0 ? 0 : MathF.PI;
        var periodFactor = 6 * MathF.PI;
        var amplitudeFactor = MathF.PI / 64.0f;
        var theta = GetAngle(angle) + amplitudeFactor * MathF.Sin(phase + periodFactor * elapsed / 1000.0f);
        p.X = dist * MathF.Cos(theta);
        p.Y = dist * MathF.Sin(theta);
        return p;
    }

    private readonly Vector2 PositionAmplitude(int elapsed, int projId, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        var dist = elapsed * (Speed / 1000f);
        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));

        var phase = projId % 2 == 0 ? 0 : MathF.PI;
        var deflection =
            _amplitude * MathF.Sin(phase + elapsed / (float)LifetimeMs * _frequency * 2 * MathF.PI);
        p.X = p.X + deflection * MathF.Cos(GetAngle(angle) + MathF.PI / 2);
        p.Y = p.Y + deflection * MathF.Sin(GetAngle(angle) + MathF.PI / 2);
        return p;
    }

    private readonly Vector2 PositionCircle(int elapsed, int projId, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        var elapsedSeconds = elapsed / 1000f;
        if (elapsedSeconds != 0)
            angle = GetAngle(angle) + Speed * elapsedSeconds * 360f.Deg2Rad();

        p.X = MathF.Cos(angle) * _radius;
        p.Y = MathF.Sin(angle) * _radius;
        return p;
    }

    private readonly Vector2 PositionBoomerang(int elapsed, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        if (elapsed > LifetimeMs / 2)
            elapsed = LifetimeMs - elapsed;
        var dist = elapsed * (Speed / 1000f);
        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));
        return p;
    }

    private readonly Vector2 PositionAccelerate(int elapsed, float angle) {
        var speed = Speed;
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        speed *= elapsed / (float)LifetimeMs;
        var dist = elapsed * (speed / 1000f);

        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));
        return p;
    }

    private readonly Vector2 PositionDecelerate(int elapsed, float angle) {
        var speed = Speed;
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        speed *= 2 - elapsed / (LifetimeMs + 10f);
        var dist = elapsed * (speed / 1000f);

        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));
        return p;
    }

    private readonly Vector2 PositionChangeSpeed(int elapsed, float angle) {
        var p = Vector2.Zero;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return p;

        elapsed -= TimeOffset;

        ApplyModifiers(ref elapsed);

        var dist = Math.Clamp(elapsed, 0, _cooldownOffset) * (Speed / 1000f); // 0 -> cooldown offset

        if (elapsed > _cooldownOffset) // cooldown offset -> end
        {
            elapsed -= _cooldownOffset;
            var increments = Math.Min(elapsed / _cooldown, _repeat);
            for (var i = 1; i <= increments; i++)
                dist += _cooldown * (Speed + i * _increment) / 1000f;
            var relElapsed = elapsed - _cooldown * increments;
            dist += relElapsed * (Speed + (increments + 1) * _increment) / 1000f;
        }

        p.X = dist * MathF.Cos(GetAngle(angle));
        p.Y = dist * MathF.Sin(GetAngle(angle));
        return p;
    }
}