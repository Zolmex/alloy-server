using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Components.Data;

namespace GameServer.Game.Entities.Behaviors.Loot;

public interface ILoot {
    void Populate(BehaviorController controller, ref Queue<Item> drops, ref DamageRecord record);
}