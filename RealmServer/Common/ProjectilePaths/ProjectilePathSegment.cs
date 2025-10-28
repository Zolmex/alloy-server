#region

using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using Common.Utilities.Net;
using System;
using System.Numerics;
using System.Xml.Linq;

#endregion

namespace Common.ProjectilePaths
{
    /// <summary>
    /// Class for a segment as part of a chained sequence of segments that form a <see cref="ProjectilePath"/>.
    /// </summary>
    public class ProjectilePathSegment
    {
        /// <summary>
        /// Gets or sets the type of Path, this is used so we can send to the client for correct parsing.
        /// </summary>
        public PathType Type { get; private set; }

        /// <summary>
        /// Gets or sets information about the projectile that controls this path segment.
        /// </summary>
        public ProjectileInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the speed of this path segment.
        /// </summary>
        public float Speed { get; }
        
        /// <summary>
        /// Gets the time offset at which the segment starts modifying the projectile's position.
        /// </summary>
        public int TimeOffset { get; }

        protected int? _lifetimeMs;

        /// <summary>
        /// How long the path segment will run for in Ms.
        /// </summary>
        public int LifetimeMs
        {
            get => _lifetimeMs ?? Info.LifetimeMs;
            set => _lifetimeMs = value;
        }

        protected float? _angle;

        /// <summary>
        /// Gets or sets the angle of this path segment.
        /// </summary>
        public float Angle
        {
            get => _angle ?? Info.ShootAngle;
            set => _angle = value;
        }

        protected readonly int _mods;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectilePathSegment"/> class.
        /// </summary>
        /// <param name="pathType">Path type.</param>
        public ProjectilePathSegment(PathType pathType, float speed, float? angle = null, int? lifetimeMs = null, int? timeOffset = null, params PathSegmentModifier[] mods)
        {
            Type = pathType;
            Speed = speed;
            TimeOffset = timeOffset ?? 0;
            _angle = angle.Deg2Rad();
            _lifetimeMs = lifetimeMs;
            _mods = GetModsFlag(mods);
        }

        public bool HasMod(PathSegmentModifier mod)
        {
            return (_mods & (1 << (int)mod)) != 0;
        }

        protected void ApplyModifiers(ref int elapsedLifetimeMs)
        {
            if (HasMod(PathSegmentModifier.Boomerang))
                if (elapsedLifetimeMs > LifetimeMs / 2)
                    elapsedLifetimeMs = LifetimeMs - elapsedLifetimeMs;
        }

        /// <summary>
        /// Gets the position offset of the projectile (relative to the start position of the segment), at a specified time relative to the start of the segment.
        /// </summary>
        /// <param name="startPos">Start position for this path.</param>
        /// <param name="elapsedLifetimeMs">Time since the start of the segment.</param>
        /// <returns>The position offset relative to the segment's start position.</returns>
        /// <exception cref="NotImplementedException">Throws exception if not implemented in a derived class.</exception>
        public virtual Vector2 PositionAt(int elapsedLifetimeMs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the position offset at the end of this segment.
        /// </summary>
        /// <returns>Position offset at the end of the segment.</returns>
        public Vector2 PositionAtEnd()
        {
            return PositionAt(LifetimeMs);
        }

        /// <summary>
        /// Writes the path data to the <see cref="NetworkWriter"/> so we can send it to the client. By default sends out Speed, LifetimeMS, and Angle.
        /// </summary>
        /// <param name="wtr">Network Writer.</param>
        public virtual void Write(NetworkWriter wtr)
        {
            wtr.Write(Speed);
            wtr.Write(LifetimeMs);
            wtr.Write(_angle ?? float.NaN);
            wtr.Write(TimeOffset);
            wtr.Write(_mods);
        }

        public virtual void SetInfo(ProjectileInfo info)
        {
            Info = info;
        }

        /// <summary>
        /// Clones itself into a new instance. Required since a behavior references a segment, and then that segment will be reused,
        /// so we need a new instance.
        /// </summary>
        /// <returns>A new instance of the segment with the same values.</returns>
        public virtual ProjectilePathSegment Clone()
        {
            return new ProjectilePathSegment(0, 0);
        }

        /// <summary>
        /// Parse a projectile desc into a projectile path.
        /// </summary>
        /// <param name="projDesc">Projectile Desc.</param>
        /// <returns>Projectile path segment.</returns>
        public static ProjectilePathSegment ParsePath(ProjectileDesc projDesc)
        {
            // No path defined, import path from old system
            ProjectilePathSegment path;
            if (projDesc.Root.HasElement("Amplitude") || projDesc.Root.HasElement("Frequency"))
            {
                path = new AmplitudePath(projDesc.Speed, projDesc.Amplitude, projDesc.Frequency, null, projDesc.LifetimeMS);
            }
            // else if (projDesc.Parametric) {
            //     path = new ParametricPath(projDesc.Speed);
            // }
            else if (projDesc.Wavy)
            {
                path = new WavyPath(projDesc.Speed, null, projDesc.LifetimeMS);
            }
            else if (projDesc.Boomerang)
            {
                path = new BoomerangPath(projDesc.Speed, null, projDesc.LifetimeMS);
            }
            else
            {
                path = new LinePath(projDesc.Speed, null, projDesc.LifetimeMS);
            }

            return path;
        }

        /// <summary>
        /// Parse an XML element into a projectile path.
        /// </summary>
        /// <param name="pathElement">XML element from file.</param>
        /// <returns>Projectile path segment.</returns>
        public static ProjectilePathSegment ParsePath(XElement pathElement)
        {
            if (pathElement == null)
                return new LinePath(10, null, 100);

            var pathName = pathElement.Value;
            var lifeTimeMs = pathElement.GetAttribute<int>("lifetimeMs");
            switch (pathName)
            {
                case "Line":
                    var speed = pathElement.GetAttribute<float>("speed");
                    return new LinePath(speed, null, lifeTimeMs);
                case "Wavy":
                    speed = pathElement.GetAttribute<float>("speed");
                    return new WavyPath(speed, null, lifeTimeMs);
                case "Boomerang":
                    speed = pathElement.GetAttribute<float>("speed");
                    return new BoomerangPath(speed, null, lifeTimeMs);
                case "Circle":
                    var rps = pathElement.GetAttribute<float>("rotationsPerSecond");
                    var radius = pathElement.GetAttribute<float>("radius");
                    return new CirclePath(rps, radius, null, lifeTimeMs);
                case "Amplitude":
                    speed = pathElement.GetAttribute<float>("speed");
                    var amplitude = pathElement.GetAttribute<float>("amplitude");
                    var frequency = pathElement.GetAttribute<float>("frequency");
                    return new AmplitudePath(speed, amplitude, frequency, null, lifeTimeMs);
                case "Accelerate":
                    speed = pathElement.GetAttribute<float>("speed");
                    return new AcceleratePath(speed, null, lifeTimeMs);
                case "Decelerate":
                    speed = pathElement.GetAttribute<float>("speed");
                    return new DeceleratePath(speed, null, lifeTimeMs);
                case "ChangeSpeed":
                    speed = pathElement.GetAttribute<float>("speed");
                    var inc = pathElement.GetAttribute<float>("inc");
                    var cooldown = pathElement.GetAttribute<int>("cooldown");
                    var cooldownOffset = pathElement.GetAttribute<int>("cooldownOffset");
                    var repeat = pathElement.GetAttribute<int>("repeat");
                    return new ChangeSpeedPath(speed, inc, cooldown, null, lifeTimeMs, cooldownOffset, repeat);
            }

            return null;
        }

        public static ProjectilePathSegment ParsePath(ProjectileProps props)
        {
            return props.PathType switch
            {
                PathType.LinePath => new LinePath(props.Speed, null, props.LifetimeMS),
                PathType.WavyPath => new WavyPath(props.Speed, null, props.LifetimeMS),
                PathType.AmplitudePath => new AmplitudePath(props.Speed, props.Amplitude, props.Frequency, null, props.LifetimeMS),
                PathType.BoomerangPath => new BoomerangPath(props.Speed, null, props.LifetimeMS),
                _ => null
            };
        }

        /// <summary>
        /// Convert a solo segment to a <see cref="ProjectilePath"/>.
        /// </summary>
        /// <returns>Projectile Path.</returns>
        public ProjectilePath ToPath()
        {
            return new ProjectilePath(LifetimeMs, this);
        }

        private static int GetModsFlag(PathSegmentModifier[] mods)
        {
            var ret = 0;
            foreach (var mod in mods)
                ret |= 1 << (int)mod;
            return ret;
        }
    }

    public struct ProjectileInfo
    {
        public Vector2 StartPos;
        public float ShootAngle;
        public int LifetimeMs;
        public int ProjId;
        public long StartTime;
    }

    public enum PathSegmentModifier : byte
    {
        None,
        Boomerang
    }
}