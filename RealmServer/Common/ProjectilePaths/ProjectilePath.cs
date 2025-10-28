#region

using Common.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    /// <summary>
    /// Movement mapping for a projectile, consisting of a collection of <see cref="ProjectilePathSegment"/>.
    /// </summary>
    public class ProjectilePath
    {
        private List<ProjectilePathSegment> projectilePathSegments = new();

        /// <summary>
        /// Gets the number of segments in this path.
        /// </summary>
        public int SegmentCount => projectilePathSegments.Count;

        /// <summary>
        /// Gets the total lifetime of this path.
        /// </summary>
        public int LifetimeMs => projectilePathSegments.Sum(p => p.LifetimeMs);

        /// <summary>
        /// Gets or sets the projectile info for this path.
        /// </summary>
        public ProjectileInfo Info { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePath"/> class.
        /// </summary>
        public ProjectilePath()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePath"/> class.
        /// </summary>
        /// <param name="baseSegment">Starting path segment/</param>
        public ProjectilePath(int lifetimeMs, ProjectilePathSegment baseSegment)
        {
            baseSegment.LifetimeMs = lifetimeMs;
            projectilePathSegments.Add(baseSegment);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePath"/> class.
        /// </summary>
        /// <param name="projectilePathSegments">Collection of segments.</param>
        public ProjectilePath(List<ProjectilePathSegment> projectilePathSegments)
        {
            foreach (var segment in projectilePathSegments)
            {
                this.projectilePathSegments.Add(segment.Clone());
            }
        }

        /// <summary>
        /// Register a segment to this path.
        /// </summary>
        /// <param name="segment">Segment.</param>
        public void RegisterSegment(ProjectilePathSegment segment)
        {
            projectilePathSegments.Add(segment);
        }

        /// <summary>
        /// Sets the projectile info for all segments in this path.
        /// </summary>
        /// <param name="info">Projectile Info.</param>
        public void SetInfo(ProjectileInfo info)
        {
            Info = info;
            foreach (var segment in projectilePathSegments)
            {
                segment.SetInfo(info);
            }
        }

        /// <summary>
        /// Calculate the position offset of a projectile on this path based on the time since the projectile started.
        /// </summary>
        /// <param name="relativeElapsed">Time since the projectile started</param>
        /// <returns>Position offset of the projectile from its start position.</returns>
        public Vector2 PositionAt(int relativeElapsed)
        {
            var segmentEnd = 0;
            var segmentsTotal = 0;
            var startPos = Vector2.Zero; // Origin
            foreach (var segment in projectilePathSegments)
            {
                segmentEnd += segment.LifetimeMs;
                if (relativeElapsed <= segmentEnd)
                {
                    var ret = segment.PositionAt(relativeElapsed - segmentsTotal); // Position offset relative to the segment start
                    return startPos + ret; // Position offset relative to the path start
                }

                startPos += segment.PositionAtEnd();
                segmentsTotal += segment.LifetimeMs;
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// Clones itself into a new instance. Required since a behavior references a path, and then that path will be reused,
        /// so we need a new instance.
        /// </summary>
        /// <returns>A new instance of the path with the same segments.</returns>
        public ProjectilePath Clone()
        {
            return new ProjectilePath(projectilePathSegments);
        }

        /// <summary>
        /// Write all of the segment data to the client so it can be parsed.
        /// </summary>
        /// <param name="wtr">Network Writer.</param>
        public void Write(NetworkWriter wtr)
        {
            wtr.Write(SegmentCount);
            foreach (var segment in projectilePathSegments)
            {
                wtr.Write((byte)segment.Type);
                segment.Write(wtr);
            }
        }
    }
}