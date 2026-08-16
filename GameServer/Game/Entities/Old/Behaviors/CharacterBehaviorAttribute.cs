namespace GameServer.Game.Entities.Old.Behaviors;

public class CharacterBehaviorAttribute : Attribute {
    public CharacterBehaviorAttribute(string objectId) {
        ObjectId = objectId;
    }

    public string ObjectId { get; }
}