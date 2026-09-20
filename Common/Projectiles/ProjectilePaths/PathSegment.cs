using System;
using System.Numerics;
using System.Xml.Linq;
using Common.Network;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;

namespace Common.Projectiles.ProjectilePaths;

public partial struct PathSegment {
    public PathType Type;
    public float Speed;
    public float FixedAngle;
    public int LifetimeMs;
    public int TimeOffset;

    private readonly int _mods;

    private float _radius; // Circle path
    
    private float _amplitude; // Amplitude path
    private float _frequency;
    
    private int _cooldown; // ChangeSpeed path
    private int _cooldownOffset;
    private float _increment;
    private int _repeat;

    public byte SubCount; // Combined path

    public PathSegment(PathType pathType, float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        Type = pathType;
        Speed = speed;
        TimeOffset = timeOffset ?? 0;
        FixedAngle = angle.Deg2Rad() ?? float.NaN;
        LifetimeMs = lifetimeMs ?? -1;
        _mods = GetModsFlag(mods);
    }

    public readonly bool HasMod(PathSegmentModifier mod) {
        return (_mods & (1 << (int)mod)) != 0;
    }

    private readonly void ApplyModifiers(ref int elapsedLifetimeMs) {
        if (HasMod(PathSegmentModifier.Boomerang)) {
            if (elapsedLifetimeMs > LifetimeMs / 2)
                elapsedLifetimeMs = LifetimeMs - elapsedLifetimeMs;
        }
    }

    public readonly Vector2 PositionAt(int elapsedLifetimeMs, int projId, float angle, ref readonly PathSegmentBuffer buffer, int selfIndex) {
        var elapsed = elapsedLifetimeMs;
        if (TimeOffset > 0 && elapsed < TimeOffset)
            return Vector2.Zero;
        
        elapsed -= TimeOffset;
        
        ApplyModifiers(ref elapsed);

        var targetAngle = GetAngle(angle);
        
        return Type switch {
            PathType.LinePath => PositionLine(elapsed, targetAngle),
            PathType.WavyPath => PositionWavy(elapsed, projId, targetAngle),
            PathType.AmplitudePath => PositionAmplitude(elapsed, projId, targetAngle),
            PathType.CirclePath => PositionCircle(elapsed, projId, targetAngle),
            PathType.BoomerangPath => PositionBoomerang(elapsed, targetAngle),
            PathType.AcceleratePath => PositionAccelerate(elapsed, targetAngle),
            PathType.DeceleratePath => PositionDecelerate(elapsed, targetAngle),
            PathType.ChangeSpeedPath => PositionChangeSpeed(elapsed, targetAngle),
            PathType.CombinedPath => PositionCombined(elapsed, projId, targetAngle, in buffer, selfIndex),
            _ => Vector2.Zero
        };
    }

    public readonly Vector2 PositionAtEnd(int projId, float angle, ref readonly PathSegmentBuffer buffer, int selfIndex) {
        return PositionAt(LifetimeMs, projId, angle, in buffer, selfIndex);
    }

    public readonly float GetAngle(float angle) {
        if (float.IsNaN(FixedAngle))
            return angle;
        return FixedAngle;
    }

    public readonly void Write(ref SpanWriter wtr) {
        wtr.Write(Speed);
        wtr.Write(LifetimeMs);
        wtr.Write(FixedAngle);
        wtr.Write(TimeOffset);
        wtr.Write(_mods);
        WritePathData(ref wtr);
    }

    public readonly void WritePathData(ref SpanWriter wtr) {
        switch (Type) {
            case PathType.CirclePath:
                wtr.Write(_radius);
                break;
            case PathType.AmplitudePath:
                wtr.Write(_amplitude);
                wtr.Write(_frequency);
                break;
            case PathType.ChangeSpeedPath:
                wtr.Write(_increment);
                wtr.Write(_cooldown);
                wtr.Write(_cooldownOffset);
                wtr.Write(_repeat);
                break;
            case PathType.CombinedPath:
                wtr.Write(TimeOffset);
                wtr.Write(_mods);
                break;
        }
    }
    
    public ProjectilePath ToPath() {
        return new ProjectilePath(LifetimeMs, this);
    }

    public static PathSegment NewLine(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.LinePath, speed, angle, lifetimeMs, timeOffset, mods);
    }
    
    public static PathSegment NewWavy(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.WavyPath, speed, angle, lifetimeMs, timeOffset, mods);
    }

    public static PathSegment NewCircle(float rps, float radius, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.CirclePath, rps, angle, lifetimeMs, timeOffset, mods) {
            _radius = radius
        };
    }

    public static PathSegment NewAmplitude(float speed, float amplitude, float frequency, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.AmplitudePath, speed, angle, lifetimeMs, timeOffset, mods) {
            _amplitude = amplitude,
            _frequency = frequency
        };;
    }
    
    public static PathSegment NewBoomerang(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.BoomerangPath, speed, angle, lifetimeMs, timeOffset, mods);
    }
    
    public static PathSegment NewAccelerate(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.AcceleratePath, speed, angle, lifetimeMs, timeOffset, mods);
    }
    
    public static PathSegment NewDecelerate(float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.DeceleratePath, speed, angle, lifetimeMs, timeOffset, mods);
    }
    
    public static PathSegment NewChangeSpeed(float speed, float inc, int cooldown, float? angle = null, int? lifetimeMs = null, int cooldownOffset = 0, int repeat = 999999, int? timeOffset = null, params PathSegmentModifier[] mods) {
        return new PathSegment(PathType.ChangeSpeedPath, speed, angle, lifetimeMs, timeOffset, mods) {
            _increment = inc,
            _cooldown = cooldown,
            _cooldownOffset = cooldownOffset,
            _repeat = repeat
        };
    }
    
    public static PathSegment NewCombined(byte subCount, int lifetimeMs, int? timeOffset = null) {
        return new PathSegment(PathType.CombinedPath, 0, null, lifetimeMs, timeOffset) {
            SubCount = subCount
        };
    }

    public static PathSegment ParsePath(ProjectileProps props) {
        return props.PathType switch {
            PathType.LinePath => NewLine(props.Speed, null, props.LifetimeMS),
            PathType.AmplitudePath => NewAmplitude(props.Speed, props.Amplitude, props.Frequency, null, props.LifetimeMS),
            PathType.WavyPath => NewWavy(props.Speed, null, props.LifetimeMS),
            PathType.BoomerangPath => NewBoomerang(props.Speed, null, props.LifetimeMS),
            _ => throw new Exception($"Path Type: {props.PathType} not supported at ParsePath(ProjectileProps).")
        };
    }
    
    public static PathSegment ParsePath(ProjectileDesc projDesc) {
        // No path defined, import path from old system
        PathSegment path;
        if (projDesc.Root.HasElement("Amplitude") || projDesc.Root.HasElement("Frequency"))
            path = NewAmplitude(projDesc.Speed, projDesc.Amplitude, projDesc.Frequency, null, projDesc.LifetimeMS);
        // else if (projDesc.Parametric) {
        //     path = new ParametricPath(projDesc.Speed);
        // }
        else if (projDesc.Wavy)
            path = NewWavy(projDesc.Speed, null, projDesc.LifetimeMS);
        else if (projDesc.Boomerang)
            path = NewBoomerang(projDesc.Speed, null, projDesc.LifetimeMS);
        else
            path = NewLine(projDesc.Speed, null, projDesc.LifetimeMS);

        return path;
    }

    public static PathSegment ParsePath(XElement pathElement) {
        if (pathElement == null)
            return NewLine(10, null, 100);

        var pathName = pathElement.Value;
        var lifeTimeMs = pathElement.GetAttribute<int>("lifetimeMs");
        switch (pathName) {
            case "Line":
                var speed = pathElement.GetAttribute<float>("speed");
                return NewLine(speed, null, lifeTimeMs);
            case "Wavy":
                speed = pathElement.GetAttribute<float>("speed");
                return NewWavy(speed, null, lifeTimeMs);
            case "Boomerang":
                speed = pathElement.GetAttribute<float>("speed");
                return NewBoomerang(speed, null, lifeTimeMs);
            case "Circle":
                var rps = pathElement.GetAttribute<float>("rotationsPerSecond");
                var radius = pathElement.GetAttribute<float>("radius");
                return NewCircle(rps, radius, null, lifeTimeMs);
            case "Amplitude":
                speed = pathElement.GetAttribute<float>("speed");
                var amplitude = pathElement.GetAttribute<float>("amplitude");
                var frequency = pathElement.GetAttribute<float>("frequency");
                return NewAmplitude(speed, amplitude, frequency, null, lifeTimeMs);
            case "Accelerate":
                speed = pathElement.GetAttribute<float>("speed");
                return NewAccelerate(speed, null, lifeTimeMs);
            case "Decelerate":
                speed = pathElement.GetAttribute<float>("speed");
                return NewDecelerate(speed, null, lifeTimeMs);
            case "ChangeSpeed":
                speed = pathElement.GetAttribute<float>("speed");
                var inc = pathElement.GetAttribute<float>("inc");
                var cooldown = pathElement.GetAttribute<int>("cooldown");
                var cooldownOffset = pathElement.GetAttribute<int>("cooldownOffset");
                var repeat = pathElement.GetAttribute<int>("repeat");
                return NewChangeSpeed(speed, inc, cooldown, null, lifeTimeMs, cooldownOffset, repeat);
        }

        throw new Exception($"Path Type: {pathName} not supported at ParsePath(XElement).");
    }

    private static int GetModsFlag(PathSegmentModifier[] mods) {
        var ret = 0;
        foreach (var mod in mods)
            ret |= 1 << (int)mod;
        return ret;
    }
}

public enum PathSegmentModifier : byte {
    None,
    Boomerang
}