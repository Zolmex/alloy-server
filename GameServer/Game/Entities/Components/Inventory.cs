using System.Runtime.CompilerServices;
using Common.Database.Models;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

[InlineArray(20)]
public struct ItemBuffer { private Item _; } // Item is a reference type but inventory isn't a hot path so we good

[InlineArray(20)]
public struct SlotTypeBuffer { private int _; }

[InlineArray(2)]
public struct PotionStackBuffer { private int _; }

[InlineArray(8)]
public struct OwnerBuffer { private int _; }

public struct Inventory {
    public int Size = 20;
    public int OwnerCount;
    
    public ItemBuffer Items;
    public SlotTypeBuffer SlotTypes;
    public PotionStackBuffer PotionStacks;
    public OwnerBuffer Owners;
    
    public BitMask256 ItemUpdates;

    public Inventory(params IEnumerable<int> slotTypes) {
        var i = 0;
        foreach (var slotType in slotTypes) {
            SlotTypes[i++] = slotType;
        }
    }
    
    public void SetItem(int slot, Item item) {
        if (slot < 0 || slot >= Size)
            return;
        
        if (item != null && SlotTypes[slot] != 0 && item.SlotType != SlotTypes[slot])
            return;
        
        Items[slot] = item;
        ItemUpdates.Set(slot);
    }
    
    public void SwapSlots(int slot1, int slot2) {
        if (slot1 < 0 || slot1 >= Size || slot2 < 0 || slot2 >= Size)
            return;
        
        (Items[slot1], Items[slot2]) = (Items[slot2], Items[slot1]);
        
        ItemUpdates.Set(slot1);
        ItemUpdates.Set(slot2);
    }

    public bool IsEquippable(Item item, int slot) {
        return SlotTypes[slot] == 0 || SlotTypes[slot] == item.SlotType;
    }

    public bool StackPotion(Item item) {
        if (item.ObjectType is not (2594 or 2595))
            return false;
        
        var stackIdx = item.ObjectType == 2594 ? 0 : 1;
        PotionStacks[stackIdx]++;
        return true;
    }
    
    public Item UnstackPotion(int slotFrom) {
        if (slotFrom is not (255 or 254))
            return null;

        var itemType = slotFrom == 255 ? 2594 : 2595;
        var item = new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root);
        var stackIdx = item.ObjectType == 2594 ? 0 : 1;
        PotionStacks[stackIdx]--;
        return item;
    }

    public bool OwnedBy(int accId) {
        if (OwnerCount == 0)
            return true;
        
        for (var i = 0; i < OwnerCount; i++)
            if (Owners[i] == accId)
                return true;
        return false;
    }
    
    public bool IsEmpty() {
        foreach (var item in Items)
            if (item != null)
                return false;
        return true;
    }
    
    public void Save(Character chr) {
        var itemDatas = new List<byte>();
        for (var i = 0; i < Size; i++) {
            var item = Items[i];
            if (item == null) {
                chr.ItemTypes[i] = -1;
                itemDatas.Add(0);
                continue;
            }
            
            chr.ItemTypes[i] = item.ObjectType;
            item.Export(itemDatas);
        }

        chr.ItemDatas = itemDatas.ToArray();
    }
}