using Common;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Components.Data;

namespace GameServer.Game.Entities.Behaviors.Loot;

public class TierLoot : ILoot {

    public TierLoot(int tier, ItemType itemType, float threshold, float chance) {
        
    }
    
    public void Populate(ref EntityContext host, ref Queue<Item> drops, ref DamageRecord record) {
        throw new NotImplementedException();
    }
}