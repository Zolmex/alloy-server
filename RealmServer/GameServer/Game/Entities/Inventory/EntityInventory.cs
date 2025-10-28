#region

using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Inventory
{
    public class EntityInventory
    {
        protected readonly Logger _log;

        protected readonly Item[] _items;

        protected readonly Entity _entity;
        public readonly object InvLock = new();

        public Item this[int slot]
        {
            get => GetItem(slot);
            set => SetItem(slot, value);
        }

        public EntityInventory(Entity entity, int numSlots)
        {
            _log = new Logger(GetType());

            _entity = entity;
            _items = new Item[numSlots];
        }

        public virtual int GetNextAvailableSlot()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                    return i;
            }

            return -1;
        }

        public void UpdateSlots(params int[] slots)
        {
            UpdateSlotsNoLock(slots);
        }

        protected void UpdateSlotsNoLock(params int[] slots)
        {
            foreach (var slot in slots)
            {
                if (_entity.IsPlayer)
                    (_entity as Player).InvChanged(slot, _items[slot]);
                _entity.Stats.Set(GetStat(slot, false), _items[slot]?.ObjectType ?? -1, _entity.IsPlayer && slot > 3);
                _entity.Stats.Set(GetStat(slot, true), _items[slot]?.ExportString(), _entity.IsPlayer && slot > 3);
            }
        }

        public void SetItem(int slot, Item item)
        {
            if (slot < 0 || slot >= _items.Length)
            {
                _log.Warn($"Inventory transaction failed for slot {slot} item {item.ObjectType}");
                return;
            }

            _items[slot] = item;

            UpdateSlotsNoLock(slot);
        }

        protected void SetSlots(Item[] items, params int[] slots)
        {
            if (slots == null || items == null || slots.Length == 0 || items.Length == 0 || slots.Length != items.Length)
            {
                _log.Warn($"Inventory transaction failed for slots {slots?.ToCommaSepString()} item {items?.Select(i => i.ObjectId).ToCommaSepString()}");
                return;
            }

            foreach (var slot in slots)
            {
                var item = items[slot];
                if (slot < 0 || slot >= items.Length)
                {
                    _log.Warn($"Inventory transaction failed for slot {slot} item {item}");
                    break;
                }

                InternalSetItem(slot, item);
                UpdateSlotsNoLock(slot);
            }
        }

        public void SetItems(Item[] items)
        {
            if (items == null || items.Length > _items.Length)
            {
                _log.Warn($"Inventory transaction failed. Items Size: {items.Length} Inv Size: {_items.Length}");
                return;
            }

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                InternalSetItem(i, item);
                UpdateSlotsNoLock(i);
            }
        }

        public void SetItems(int[] itemTypes, byte[] itemDatas)
        {
            if (itemTypes == null || itemDatas == null || itemTypes.Length > _items.Length)
            {
                _log.Warn($"Inventory transaction failed. Items Size: {itemTypes.Length} Item Datas Size: {itemDatas?.Length ?? -1} Inv Size: {_items.Length}");
                return;
            }

            using var stream = new MemoryStream(itemDatas);
            using var reader = new BinaryReader(stream);
            for (var i = 0; i < itemTypes.Length; i++)
            {
                Item item = null;
                var itemType = itemTypes[i];
                if (itemType != -1)
                {
                    var xml = XmlLibrary.ItemDescs[(ushort)itemType].Root;
                    item = new Item(xml);
                    item.Import(reader);
                }
                else
                    reader.ReadByte(); // 0 byte

                InternalSetItem(i, item);
                UpdateSlotsNoLock(i);
            }
        }

        public void SetItem(int slot, int itemType, BinaryReader dataReader)
        {
            if (slot < 0 || slot >= _items.Length)
            {
                _log.Warn($"Inventory transaction failed for slot {slot} item {itemType}");
                return;
            }

            Item item = null;
            if (itemType != -1)
            {
                var xml = XmlLibrary.ItemDescs[(ushort)itemType].Root;
                item = new Item(xml);
                item.Import(dataReader);
            }
            else
                dataReader.ReadByte(); // 0 byte

            InternalSetItem(slot, item);
            UpdateSlotsNoLock(slot);
        }

        public virtual Item RemoveItem(byte slotId)
        {
            if (slotId > _items.Length)
                return null;

            var item = _items[slotId];
            SetItemNoLock(null, slotId);
            return item;
        }

        public void SwapSlots(byte slot1, byte slot2)
        {
            var item = _items[slot1];
            var item2 = _items[slot2];

            SetItemNoLock(item2, slot1);
            SetItemNoLock(item, slot2);
        }

        // This will ALWAYS swap two items, so perform any validation BEFORE you call it
        public static void InventorySwap(EntityInventory inv1, EntityInventory inv2, byte inv1SlotId, byte inv2SlotId)
        {
            var item = inv1.GetItemNoLock(inv1SlotId);
            var otherItem = inv2.GetItemNoLock(inv2SlotId);

            inv1.SetItemNoLock(otherItem, inv1SlotId);
            inv2.SetItemNoLock(item, inv2SlotId);
        }

        public static void InventorySwap(EntityInventory inv, byte slot1Id, byte slot2Id)
        {
            var item = inv.GetItemNoLock(slot1Id);
            var otherItem = inv.GetItemNoLock(slot2Id);

            inv.SetItemNoLock(otherItem, slot1Id);
            inv.SetItemNoLock(item, slot2Id);
        }

        public int[] GetItemTypes()
        {
            var ret = new int[_items.Length];
            for (var i = 0; i < ret.Length; i++)
                ret[i] = _items[i]?.ObjectType ?? -1;
            return ret;
        }

        public byte[] GetItemDatas()
        {
            var data = new List<byte>();
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                if (item == null)
                    data.Add(0);
                else
                    data = item.Export(data);
            }

            return data.ToArray();
        }

        public virtual Item GetItem(int slot)
        {
            if (slot < 0 || slot > _items.Length)
                return null;

            return _items[slot];
        }

        public IEnumerable<Item> GetItems()
        {
            foreach (var item in _items)
                yield return item;
        }

        // DO NOT USE THIS IF YOU HAVE NOT LOCKED THE INVENTORY.
        public Item GetItemNoLock(int slot)
        {
            return _items[slot];
        }

        // DO NOT USE THIS IF YOU HAVE NOT LOCKED THE INVENTORY.
        public void SetItemNoLock(Item item, int slot)
        {
            InternalSetItem(slot, item);
            UpdateSlotsNoLock(slot);
        }

        protected void InternalSetItem(int slot, Item item)
        {
            _items[slot] = item;
        }

        public bool IsEmpty()
        {
            foreach (var item in _items)
                if (item != null)
                    return false;
            return true;
        }

        public static EntityInventory GetInventoryById(World world, int objId)
        {
            if (!world.Entities.TryGetValue(objId, out var targetEntity))
                return null;

            if (targetEntity is Player p)
                return p.Inventory;
            else if (targetEntity is Container c)
                return c.Inventory;

            return null;
        }

        public static ushort GetBagIdFromType(BagType bagType)
        {
            switch (bagType)
            {
                case BagType.Common:
                    return 1280;
                case BagType.Pink:
                    return 1286;
                case BagType.Cyan:
                    return 1288;
                case BagType.Blue:
                    return 1289;
                case BagType.White:
                    return 1296;
                case BagType.Purple:
                    return 1287;
            }

            return 1280;
        }

        public enum BagType
        {
            Common = 0,
            Pink = 1,
            Cyan = 2,
            Blue = 3,
            White = 4,
            Purple = 5
        }

        protected StatType GetStat(int slot, bool itemData)
        {
            switch (slot)
            {
                case 0: return itemData ? StatType.InventoryData0 : StatType.Inventory0;
                case 1: return itemData ? StatType.InventoryData1 : StatType.Inventory1;
                case 2: return itemData ? StatType.InventoryData2 : StatType.Inventory2;
                case 3: return itemData ? StatType.InventoryData3 : StatType.Inventory3;
                case 4: return itemData ? StatType.InventoryData4 : StatType.Inventory4;
                case 5: return itemData ? StatType.InventoryData5 : StatType.Inventory5;
                case 6: return itemData ? StatType.InventoryData6 : StatType.Inventory6;
                case 7: return itemData ? StatType.InventoryData7 : StatType.Inventory7;
                case 8: return itemData ? StatType.InventoryData8 : StatType.Inventory8;
                case 9: return itemData ? StatType.InventoryData9 : StatType.Inventory9;
                case 10: return itemData ? StatType.InventoryData10 : StatType.Inventory10;
                case 11: return itemData ? StatType.InventoryData11 : StatType.Inventory11;
                case 12: return itemData ? StatType.InventoryData12 : StatType.Backpack0;
                case 13: return itemData ? StatType.InventoryData13 : StatType.Backpack1;
                case 14: return itemData ? StatType.InventoryData14 : StatType.Backpack2;
                case 15: return itemData ? StatType.InventoryData15 : StatType.Backpack3;
                case 16: return itemData ? StatType.InventoryData16 : StatType.Backpack4;
                case 17: return itemData ? StatType.InventoryData17 : StatType.Backpack5;
                case 18: return itemData ? StatType.InventoryData18 : StatType.Backpack6;
                case 19: return itemData ? StatType.InventoryData19 : StatType.Backpack7;
                default:
                    throw new ArgumentException($"Invalid inventory slot {slot}");
            }
        }
    }
}