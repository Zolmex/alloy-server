using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Components.Data;

namespace GameServer.Game.Entities.Behaviors.Loot;

public interface ILoot {
    void Populate(ref EntityView host, ref List<Item> drops, ref DamageRecord record);
}