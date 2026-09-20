using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Common.Network;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;

namespace Common.Projectiles.ProjectilePaths;

[InlineArray(ProjectilePath.MAX_SEGMENTS)]
public struct PathSegmentBuffer {
    private PathSegment _element0;
}

public struct ProjectilePath {
    public const int MAX_SEGMENTS = 6; // Increase this if you want to use more than 6 paths in a single projectile, psychopath.
    
    private static readonly Logger _log = new(typeof(ProjectilePath));
    
    public PathSegmentBuffer Segments;
    public byte SegmentCount;

    public readonly int LifetimeMs {
        get {
            var total = 0;
            for (var i = 0; i < SegmentCount; i++) {
                var segment = Segments[i];
                total += segment.LifetimeMs;
                if (segment.Type == PathType.CombinedPath)
                    i += segment.SubCount;
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
        if (SegmentCount < MAX_SEGMENTS) {
            Segments[SegmentCount] = segment;
            SegmentCount++;
        }
    }
    
    public void RegisterCombined(int timeOffset, params PathSegment[] children) {
        if (SegmentCount + children.Length > MAX_SEGMENTS)
            return;
        
        var lifetimeMs = children.Max(c => c.TimeOffset + c.LifetimeMs);
        Segments[SegmentCount++] = PathSegment.NewCombined((byte)children.Length, lifetimeMs, timeOffset);
        foreach (var child in children) {
            if (child.Type == PathType.CombinedPath)
                _log.Warn("Sub-segments can't be of type CombinedPath.");
            Segments[SegmentCount++] = child;
        }
    }

    public readonly Vector2 PositionAt(int relativeElapsed, int projId, float angle) {
        var segmentEnd = 0;
        var segmentsTotal = 0;
        var startPos = Vector2.Zero;

        for (var i = 0; i < SegmentCount; i++) {
            ref readonly var segment = ref Segments[i];
            segmentEnd += segment.LifetimeMs;

            if (relativeElapsed <= segmentEnd)
                return startPos + segment.PositionAt(relativeElapsed - segmentsTotal, projId, angle, in Segments, i);

            startPos += segment.PositionAtEnd(projId, angle, in Segments, i);
            segmentsTotal += segment.LifetimeMs;

            if (segment.Type == PathType.CombinedPath)
                i += segment.SubCount;
        }

        return Vector2.Zero;
    }

    public readonly void Write(ref SpanWriter wtr) {
        wtr.Write(SegmentCount);
        for (var i = 0; i < SegmentCount; i++) {
            var segment = Segments[i];
            wtr.Write((byte)segment.Type);

            if (segment.Type == PathType.CombinedPath) {
                wtr.Write(segment.SubCount);
                for (var j = 0; j < segment.SubCount; j++) {
                    var sub = Segments[i + 1 + j];
                    wtr.Write((byte)sub.Type);
                    sub.Write(ref wtr);
                }
                segment.WritePathData(ref wtr);
                i += segment.SubCount;
                continue;
            }
            
            segment.Write(ref wtr);
        }
    }
}