#region

using Common.Utilities;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    /// <summary>
    /// Base class for all BehaviorTransitions, a collection of classes that perform common state transiational logic on a given entity.
    /// These are used in tandem with an <see cref="EntityBehavior"/> to form the behavior system.
    /// </summary>
    public class BehaviorTransition(TransitionType transitionType = TransitionType.Random) : IStateChild
    {
        private List<string> targetStates = new();
        private List<string> usedStates = new();

        private TransitionType TransitionType => transitionType;

        /// <summary>
        /// Register states that can be transitioned to when the transition condition is met.
        /// </summary>
        /// <param name="targetStates">States that can be transitioned to.</param>
        public void RegisterTargetStates(params string[] targetStates)
        {
            foreach (var targetState in targetStates)
            {
                this.targetStates.Add(targetState);
            }
        }

        /// <summary>
        /// Gets a state that can be transitioned to.
        /// </summary>
        /// <returns>The state that can be transitioned to.</returns>
        public string GetTargetState()
        {
            if (usedStates.Count == targetStates.Count)
            {
                usedStates.Clear();
            }

            switch (TransitionType)
            {
                case TransitionType.Random:
                    return targetStates.RandomElement();
                case TransitionType.Random7Bag:
                    var targetState = targetStates.Except(usedStates).RandomElement();
                    usedStates.Add(targetState);
                    return targetState;
                case TransitionType.Sequential:
                    targetState = targetStates[usedStates.Count];
                    usedStates.Add(targetState);
                    return targetState;
            }

            return null; // never happens but compiler is loud.
        }

        /// <summary>
        /// Function that will contain any logic for starting a transition, for anything that is required to be initialized before the transition runs on tick.
        /// </summary>
        /// <param name="host">The character that the behavior is being ran on.</param>
        public virtual void Start(Character host)
        { }

        /// <summary>
        /// Function that will contain any logic for running the transition on a tick by tick basis.
        /// </summary>
        /// <param name="host">The character that the transition is being ran on.</param>
        /// <param name="time">The current <see cref="RealmTime"/> the transition was ticked at.</param>
        /// <returns>Returns the state to be transitioned to if the transition condition is met, or null if the condition is not met.</returns>
        public virtual string Tick(Character host, RealmTime time)
        {
            return null;
        }

        /// <summary>
        /// Function that will contain any logic for stopping a transition, for anything that is required to be ran after the transition is done being ticked in a state.
        /// </summary>
        /// <param name="host">The character that the transition is being ran on.</param>
        /// <param name="time">The current <see cref="RealmTime"/> the transition is being ended at.</param>
        public virtual void End(Character host, RealmTime time)
        { }
    }
}