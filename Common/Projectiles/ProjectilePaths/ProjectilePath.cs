using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Common.Network;

namespace Common.Projectiles.ProjectilePaths;

[InlineArray(10)] // Increase this if you want to use more than 10 paths in a single projectile, psychopath.
public struct PathSegmentBuffer {
    private PathSegment _element0;
}

public struct ProjectilePath {
    public PathSegmentBuffer Segments;
    public byte SegmentCount;

    public readonly int LifetimeMs {
        get {
            var total = 0;
            for (var i = 0; i < SegmentCount; i++) {
                total += Segments[i].LifetimeMs;
            }
            return total;
        }
    }

    public ProjectilePath(int lifetimeMs, PathSegment baseSegment) {
        Segments = default;
        baseSegment.LifetimeMs = lifetimeMs;
        Segments[0] = baseSegment;
        SegmentCount = 1;
    }

    public void RegisterSegment(PathSegment segment) {
        if (SegmentCount < 4) {
            Segments[SegmentCount] = segment;
            SegmentCount++;
        }
    }

    public readonly Vector2 PositionAt(int relativeElapsed, int projId, float angle) {
        var segmentEnd = 0;
        var segmentsTotal = 0;
        var startPos = Vector2.Zero;

        for (var i = 0; i < SegmentCount; i++) {
            ref readonly var segment = ref Segments[i];
            segmentEnd += segment.LifetimeMs;
            
            if (relativeElapsed <= segmentEnd) {
                var ret = segment.PositionAt(relativeElapsed - segmentsTotal, projId, angle);
                return startPos + ret;
            }

            startPos += segment.PositionAtEnd(projId, angle);
            segmentsTotal += segment.LifetimeMs;
        }

        return Vector2.Zero;
    }

    public readonly void Write(ref SpanWriter wtr) {
        wtr.Write(SegmentCount);
        for (var i = 0; i < SegmentCount; i++) {
            wtr.Write((byte)Segments[i].Type);
            Segments[i].Write(ref wtr);
        }
    }
}