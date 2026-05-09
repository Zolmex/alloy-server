#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.Network;

#endregion

namespace Common.Projectiles.ProjectilePaths;

public class ProjectilePath {
    public int SegmentCount => projectilePathSegments.Count;
    public int LifetimeMs => projectilePathSegments.Sum(p => p.LifetimeMs);

    private readonly List<ProjectilePathSegment> projectilePathSegments = new();

    public ProjectilePath() { }

    public ProjectilePath(int lifetimeMs, ProjectilePathSegment baseSegment) {
        baseSegment.LifetimeMs = lifetimeMs;
        projectilePathSegments.Add(baseSegment);
    }

    public ProjectilePath(List<ProjectilePathSegment> projectilePathSegments) {
        foreach (var segment in projectilePathSegments)
            this.projectilePathSegments.Add(segment.Clone());
    }

    public void RegisterSegment(ProjectilePathSegment segment) {
        projectilePathSegments.Add(segment);
    }

    public Vector2 PositionAt(int relativeElapsed, int projId, float angle) {
        var segmentEnd = 0;
        var segmentsTotal = 0;
        var startPos = Vector2.Zero; // Origin
        foreach (var segment in projectilePathSegments) {
            segmentEnd += segment.LifetimeMs;
            if (relativeElapsed <= segmentEnd) {
                var ret = segment.PositionAt(relativeElapsed -
                                             segmentsTotal, projId, angle); // Position offset relative to the segment start
                return startPos + ret; // Position offset relative to the path start
            }

            startPos += segment.PositionAtEnd(projId, angle);
            segmentsTotal += segment.LifetimeMs;
        }

        return Vector2.Zero;
    }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(SegmentCount);
        foreach (var segment in projectilePathSegments) {
            wtr.Write((byte)segment.Type);
            segment.Write(ref wtr);
        }
    }
}