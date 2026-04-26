#region

using System;

#endregion

namespace GameServer.Game.Entities.Behaviors;

public class CharacterBehaviorAttribute : Attribute {
    public CharacterBehaviorAttribute(string objectId) {
        ObjectId = objectId;
    }

    public string ObjectId { get; }
}