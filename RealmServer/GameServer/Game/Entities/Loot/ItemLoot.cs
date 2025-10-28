#region

using Common.Resources.Xml;

#endregion

namespace GameServer.Game.Entities.Loot
{
    public class ItemLoot
    {
        public ushort ItemType { get; }
        public float DropChance { get; }
        public float Threshold { get; }

        public ItemLoot(ushort itemType, float dropChance, float threshold = 0f)
        {
            ItemType = itemType;
            DropChance = dropChance;
            Threshold = threshold;
        }

        public ItemLoot(string itemName, float dropChance, float threshold = 0f)
        {
            ItemType = XmlLibrary.Id2Item(itemName).ObjectType;
            DropChance = dropChance;
            Threshold = threshold;
        }
    }
}