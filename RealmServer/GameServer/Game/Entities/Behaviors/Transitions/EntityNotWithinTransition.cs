#region

using Common.Utilities;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Transitions
{
    #region

    using static BehaviorScript;

    #endregion

    /// <summary>
    /// A class for transitioning to a given state after a target is not within a radius.
    /// </summary>
    public class EntityNotWithinTransition : BehaviorTransition
    {
        private readonly string target;
        private readonly float radius;
        private readonly TargetType targetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotWithinTransition"/> class.
        /// </summary>
        /// <param name="xml">Behavior XML.</param>
        public EntityNotWithinTransition(XElement xml)
        {
            RegisterTargetStates(xml.GetAttribute<string>("targetStates").Split(','));
            target = xml.GetAttribute<string>("target");
            radius = xml.GetAttribute<float>("radius");
            targetType = target == "player" ? TargetType.Player : TargetType.Entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotWithinTransition"/> class.
        /// </summary>
        /// <param name="targetState">Target state to transition to.</param>
        /// <param name="target">"player" if the target should be a player, or name of the entity if the target is an entity.</param>
        /// <param name="radius">Radius within which the transition should be considered completed if the entity is not found.</param>
        /// <param name="transitionType">How the transition should decide which state to transition to.</param>
        public EntityNotWithinTransition(string targetState, string target = "player", float radius = 8f, TransitionType transitionType = TransitionType.Random)
            : base(transitionType)
        {
            RegisterTargetStates(targetState);
            this.target = target;
            this.radius = radius;
            targetType = this.target == "player" ? TargetType.Player : TargetType.Entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotWithinTransition"/> class.
        /// </summary>
        /// <param name="target">"player" if the target should be a player, or name of the entity if the target is an entity.</param>
        /// <param name="radius">Radius within which the transition should be considered completed if the entity is not found.</param>
        /// <param name="transitionType">How the transition should decide which state to transition to.</param>
        /// <param name="targetStates">Target states that can be transitioned to. If multiple are set, it will pick a random state.</param>
        public EntityNotWithinTransition(string target = "player", float radius = 8f, TransitionType transitionType = TransitionType.Random, params string[] targetStates)
            : base(transitionType)
        {
            RegisterTargetStates(targetStates);
            this.target = target;
            this.radius = radius;
            targetType = this.target == "player" ? TargetType.Player : TargetType.Entity;
        }

        /// <inheritdoc/>
        public override string Tick(Character host, RealmTime time)
        {
            if (targetType == TargetType.Player && !host.GetPlayersWithin(radius).Any())
            {
                return GetTargetState();
            }
            else if (targetType == TargetType.Entity && !host.GetOtherEnemiesByName(target, radius).Any())
            {
                return GetTargetState();
            }

            return null;
        }
    }
}