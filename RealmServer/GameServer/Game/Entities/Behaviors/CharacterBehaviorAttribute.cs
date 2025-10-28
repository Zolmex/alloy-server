#region

using System;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class CharacterBehaviorAttribute : Attribute
    {
        public string ObjectId { get; }

        public CharacterBehaviorAttribute(string objectId)
        {
            ObjectId = objectId;
        }
    }
}