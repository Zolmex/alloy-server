using System;
using System.Buffers;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityInventory : IEntityIdentifiable, IDisposable {
    public EntityId Id { get; set; }

    public int OwnerAccountId;
    public Item this[int slot] {
        get {
            if (slot < 0 || slot >= _items.Length)
                return null;
            return _items[slot];
        }
    }

    private readonly World _world;
    private readonly int _size;
    private readonly int[] _slotTypes;
    private readonly Item[] _items;
    private readonly int[] _potionStacks;
    private BitMask256 _itemUpdates;

    public EntityInventory(World world, ref Entity en, int size) {
        Id = en.Id;
        _world = world;
        _size = size;
        OwnerAccountId = -1;
        
        _slotTypes = ArrayPool<int>.Shared.Rent(size);
        Array.Fill(_slotTypes, 0);
        _items = ArrayPool<Item>.Shared.Rent(size);
        Array.Fill(_items, null);
        _potionStacks = ArrayPool<int>.Shared.Rent(2);
        Array.Fill(_potionStacks, 0);
    }

    public void Init(Span<int> slotTypes, Span<int> itemTypes) {
        slotTypes.CopyTo(_slotTypes);
        for (var i = 0; i < itemTypes.Length; i++) {
            var itemType = itemTypes[i];
            if (itemType == -1)
                continue;

            _items[i] = new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root);
        }
    }

    public void SetItem(int slot, Item item) {
        if (slot < 0 || slot >= _size)
            return;

        if (item != null && _slotTypes[slot] != 0 && item.SlotType != _slotTypes[slot])
            return;
        
        _items[slot] = item;
        _itemUpdates.Set(slot);
    }

    public bool IsEmpty() {
        foreach (var item in _items)
            if (item != null)
                return false;
        return true;
    }
    
    public bool IsEquippable(Item item, int slot) {
        var slotType = _slotTypes[slot];
        return slotType == 0 || slotType == item.SlotType;
    }

    public bool StackPotion(Item item) {
        if (item.ObjectType is not (2594 or 2595))
            return false;
        
        var stackIdx = item.ObjectType == 2594 ? 0 : 1;
        _potionStacks[stackIdx]++;
        return true;
    }

    public Item UnstackPotion(int slotFrom) {
        if (slotFrom is not (255 or 254))
            return null;

        var itemType = slotFrom == 255 ? 2594 : 2595;
        var item = new Item(XmlLibrary.ItemDescs[(ushort)itemType].Root);
        var stackIdx = item.ObjectType == 2594 ? 0 : 1;
        _potionStacks[stackIdx]--;
        return item;
    }

    public void SwapSlots(int slot1, int slot2) {
        if (slot1 < 0 || slot1 >= _size || slot2 < 0 || slot2 >= _size)
            return;
        
        var temp = _items[slot1];
        _items[slot1] = _items[slot2];
        _items[slot2] = temp;
        _itemUpdates.Set(slot1);
        _itemUpdates.Set(slot2);
    }
    
    public void Tick(ref RealmTime time) {
        ref var stats = ref _world.EntityStats.Get(Id);
        if (stats.Id == EntityId.Null || _itemUpdates.IsEmpty)
            return;
        
        stats.Set(StatType.HealthPotionStack, _potionStacks[0]);
        stats.Set(StatType.MagicPotionStack, _potionStacks[1]);
        for (var i = 0; i < _size; i++)
            if (_itemUpdates.IsSet(i)) {
                stats.Set(StatType.Inventory0 + i, _items[i]?.ObjectType ?? -1);
                stats.Set(StatType.InventoryData0 + i, _items[i]?.ExportString());
            }

        _itemUpdates.Clear();
    }

    public void Dispose() {
        ArrayPool<int>.Shared.Return(_slotTypes);
        ArrayPool<Item>.Shared.Return(_items);
    }
}