using System;
using System.Buffers;
using Common;
using Common.Game;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Worlds;

namespace GameServer.Game.Entities.Components;

public struct EntityInventory : IIdentifiable, IDisposable {
    public int Id { get; set; }

    private readonly World _world;
    private readonly int _size;
    private readonly int[] _slotTypes;
    private readonly Item[] _items;
    private BitMask256 _itemUpdates;

    public EntityInventory(World world, ref Entity en, int size) {
        Id = en.Id;
        _world = world;

        _size = size;
        _slotTypes = ArrayPool<int>.Shared.Rent(size);
        _items = ArrayPool<Item>.Shared.Rent(size);
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
    
    public void Tick(ref RealmTime time) {
        ref var stats = ref _world.EntityStats.Get(Id);
        if (stats.Id == 0 || _itemUpdates.IsEmpty)
            return;
        
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