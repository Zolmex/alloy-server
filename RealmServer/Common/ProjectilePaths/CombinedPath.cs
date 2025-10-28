#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities.Net;
using System;
using System.Linq;
using System.Numerics;

#endregion

namespace Common.ProjectilePaths
{
    public class CombinedPath : ProjectilePathSegment
    {
        private readonly ProjectilePathSegment[] _segments;
        
        public CombinedPath(int? timeOffset = null, params ProjectilePathSegment[] segments)
            : base(PathType.CombinedPath, 0, timeOffset: timeOffset)
        {
            _segments = segments;

            this._lifetimeMs = segments.Max(i => i.TimeOffset + i.LifetimeMs);
        }

        public override Vector2 PositionAt(int elapsedLifetimeMs)
        {
            var p = Vector2.Zero;
            if (TimeOffset > 0 && elapsedLifetimeMs < TimeOffset)
                return p;

            elapsedLifetimeMs -= TimeOffset;
            
            ApplyModifiers(ref elapsedLifetimeMs);
            
            var deltaX = 0f;
            var deltaY = 0f;

            var count = 0;
            foreach (var segment in _segments)
            {
                if (segment.TimeOffset > 0 && elapsedLifetimeMs < segment.TimeOffset)
                    continue;
                
                var segmentOffset = segment.PositionAt(elapsedLifetimeMs);
                deltaX += segmentOffset.X;
                deltaY += segmentOffset.Y;
                count++;
            }

            p.X = deltaX / count; // Return average deltaX and deltaY
            p.Y = deltaY / count;
            return p;
        }

        public override void Write(NetworkWriter wtr)
        {
            wtr.Write((byte)_segments.Length);
            foreach (var segment in _segments)
            {
                wtr.Write((byte)segment.Type);
                segment.Write(wtr);
            }
            wtr.Write(TimeOffset);
            wtr.Write(_mods);
        }

        public override void SetInfo(ProjectileInfo info)
        {
            base.SetInfo(info);
            foreach (var segment in _segments)
            {
                segment.SetInfo(info);
            }
        }

        public override ProjectilePathSegment Clone()
        {
            return new CombinedPath(TimeOffset, (ProjectilePathSegment[])_segments.Clone());
        }
    }
}