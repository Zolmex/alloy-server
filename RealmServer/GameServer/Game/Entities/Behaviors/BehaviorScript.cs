namespace GameServer.Game.Entities.Behaviors
{
    /// <summary>
    /// Base class for all BehaviorScripts, a collection of classes that perform common behavior logic on a given entity.
    /// These are used in tandem with an <see cref="EntityBehavior"/> to form the behavior system.
    /// </summary>
    public record BehaviorScript : IStateChild
    {
        /// <summary>
        /// A return from the <see cref="Tick(Character, RealmTime)"/> function to indicate what happened during the tick.
        /// </summary>
        public enum BehaviorTickState
        {
            /// <summary>
            /// The behavior was on cooldown during the tick.
            /// </summary>
            OnCooldown,

            /// <summary>
            /// The behavior failed to be ran during the tick.
            /// </summary>
            BehaviorFailed,

            /// <summary>
            /// The behavior was sucesssfully ran this tick.
            /// </summary>
            BehaviorActive,

            /// <summary>
            /// The behavior was successfully ran for the first time this tick.
            /// </summary>
            BehaviorActivate,

            /// <summary>
            /// The behavior was successfully ran, and met the deactivation condition for the behavior this tick.
            /// </summary>
            BehaviorDeactivate
        }

        /// <summary>
        /// An enum defining how a <see cref="BehaviorScript"/> will acquire the its target for the behavior.
        /// </summary>
        public enum TargetType
        {
            /// <summary>
            /// The behavior will target the closest player.
            /// </summary>
            ClosestPlayer,

            /// <summary>
            /// The behavior will use a fixed angle.
            /// </summary>
            FixedAngle,

            /// <summary>
            /// The behavior will target a random player each time it runs.
            /// </summary>
            RandomPlayerPerBehavior,

            /// <summary>
            /// The behavior will target a random player each time it completes a behavior cycle.
            /// </summary>
            RandomPlayerPerCycle,

            /// <summary>
            /// The behavior will target the farthest player.
            /// </summary>
            FarthestPlayer,

            /// <summary>
            /// The behavior will be targetting a player.
            /// </summary>
            Player,

            /// <summary>
            /// The behavior will be targeting an entity.
            /// </summary>
            Entity
        }

        /// <summary>
        /// Function that will contain any logic for starting a behavior, for anything that is required to be initialized before the behavior runs on tick.
        /// Useful for resetting variables like cooldowns and angles.
        /// </summary>
        /// <param name="host">The character that the behavior is being ran on.</param>
        public virtual void Start(Character host)
        { }

        /// <summary>
        /// Function that will contain any logic for running the behavior on a tick by tick basis.
        /// </summary>
        /// <param name="host">The character that the behavior is being ran on.</param>
        /// <param name="time">The current <see cref="RealmTime"/> the behavior was ticked at.</param>
        /// <returns>Returns a <see cref="BehaviorTickState"/> which will indicate what happened during the tick.</returns>
        public virtual BehaviorTickState Tick(Character host, RealmTime time)
        {
            return BehaviorTickState.OnCooldown;
        }

        /// <summary>
        /// Function that will contain any logic for stopping a behavior, for anything that is required to be ran after the behavior is done being ticked in a state.
        /// </summary>
        /// <param name="host">The character that the behavior is being ran on.</param>
        /// <param name="time">The current <see cref="RealmTime"/> the behavior is being ended at.</param>
        public virtual void End(Character host, RealmTime time)
        { }
    }
}