namespace GameServer.Game.Entities.Behaviors;

public enum TransitionType {
    Random,
    Random7Bag, // Randomly choose a state from the list, but it must use all of the states before a state can be chosen again.
    Sequential
}