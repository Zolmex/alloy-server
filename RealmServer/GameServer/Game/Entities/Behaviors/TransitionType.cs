namespace GameServer.Game.Entities.Behaviors
{
    /// <summary>
    /// An enum defining how a <see cref="BehaviorTransition"/> will select its target state.
    /// </summary>
    public enum TransitionType
    {
        /// <summary>
        /// Randomly choose a state from the list.
        /// </summary>
        Random,

        /// <summary>
        /// Randomly choose a state from the list, but it must use all of the states before a state can be chosen again.
        /// </summary>
        Random7Bag,

        /// <summary>
        /// States will be selected sequentially.
        /// </summary>
        Sequential
    }
}