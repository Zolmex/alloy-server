#region

using System;

#endregion

namespace GameServerOld.Game.Entities.Behaviors;

public class CharacterBehaviorAttribute : Attribute {
    public CharacterBehaviorAttribute(string objectId) {
        ObjectId = objectId;
    }

    public string ObjectId { get; }
}