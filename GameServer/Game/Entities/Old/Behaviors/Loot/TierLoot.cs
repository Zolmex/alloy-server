using Common;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Old.Components.Data;

namespace GameServer.Game.Entities.Old.Behaviors.Loot;

public class TierLoot : ILoot {

    public TierLoot(int tier, ItemType itemType, float threshold, float chance) {
        
    }
    
    public void Populate(ref EntityView host, ref Queue<Item> drops, ref DamageRecord record) {
        throw new NotImplementedException();
    }
}