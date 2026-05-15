using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Components.Data;

namespace GameServer.Game.Entities.Behaviors.Loot;

public class ItemLoot : ILoot {

    private readonly string _objectId;
    private readonly float _threshold;
    private readonly float _chance;
    
    public ItemLoot(string objectId, float threshold, float chance) {
        _objectId = objectId;
        _threshold = threshold;
        _chance = chance;
    }
    
    public void Populate(ref EntityView host, ref List<Item> drops, ref DamageRecord record) {
        if ((float)record.DamageDealt / host.Stats.GetInt(StatType.MaxHP) < _threshold)
            return;
        
        if (Random.Shared.NextDouble() > _chance)
            return;
        
        drops.Add(new Item(XmlLibrary.Id2Item(_objectId).Root));
    }
}