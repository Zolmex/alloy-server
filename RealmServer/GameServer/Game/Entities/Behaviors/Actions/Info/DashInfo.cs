#region

using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    /// <summary>
    /// Information storage class for any runtime properties required by the <see cref="Dash"/> behavior.
    /// </summary>
    public class DashInfo
    {
        /// <summary>
        /// Gets or sets how many times the behavior will Dash per cycle.
        /// </summary>
        public int DashCount { get; set; }

        /// <summary>
        /// Gets or sets a fixed angle in radians set at the beginning of the dash, that will be used for the duration of the dash.
        /// </summary>
        public float DashAngle { get; set; }

        /// <summary>
        /// Gets or sets the ID for current target for this dash.
        /// </summary>
        public int CurrentTargetID { get; set; }

        /// <summary>
        /// Gets or sets how long the behavior should be on cooldown for after each dash.
        /// </summary>
        public int DashCooldown { get; set; }

        /// <summary>
        /// Gets or sets how long the behavior should be on cooldown for after each dash cycle.
        /// </summary>
        public int CycleCooldown { get; set; }

        /// <summary>
        /// Gets or sets the time in ticks in which the dash started.
        /// </summary>
        public long DashStarted { get; set; }

        /// <summary>
        /// Gets or sets the position at which the dash started.
        /// </summary>
        public Vector2 DashStartPos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dash is currently being ran.
        /// </summary>
        public bool Dashing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a dash is currently in a cycle.
        /// </summary>
        public bool InCycle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we have sent the BehaviorTickState.Start for the currently running dash.
        /// </summary>
        public bool DashStartSent { get; set; }

        /// <summary>
        /// Gets or sets a collection of all of the entities that have been hit for the currently running dash.
        /// </summary>
        public HashSet<Entity> HitThisDash { get; set; } = new();
    }
}