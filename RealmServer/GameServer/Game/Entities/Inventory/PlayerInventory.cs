#region

using Common.Database;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace GameServer.Game.Entities.Inventory
{
    public class PlayerInventory : EntityInventory
    {
        private const int MAX_INV_SLOTS = 20;
        private const int HP_SLOT = 254;
        private const int MP_SLOT = 255;
        private const int MAX_POTIONS = 6;
        
        private static readonly Dictionary<int, Item> _potionStack = new ();

        private readonly Player _player;
        
        private int _healthPotionCount;
        private int _magicPotionCount;

        private int _invSize;

        public PlayerInventory(Player player)
            : base(player, MAX_INV_SLOTS)
        {
            _player = player;
            if (_potionStack.Count == 0) // Populate potion stack
            {
                _potionStack.TryAdd(HP_SLOT, new Item(XmlLibrary.ItemDescs[2594].Root));
                _potionStack.TryAdd(MP_SLOT, new Item(XmlLibrary.ItemDescs[2595].Root));
            }
        }

        public override Item GetItem(int slot)
        {
            if (slot is 254 or 255)
                return _potionStack[slot];
            return base.GetItem(slot);
        }

        public override Item RemoveItem(byte slotId)
        {
            if (slotId is 254 or 255)
            {
                var item = _potionStack[slotId];
                if (!RemovePotionStack(item.ObjectType))
                    return null;
                return item;
            }

            return base.RemoveItem(slotId);
        }

        public bool HandlePotionSwap(Item item, Item item2, byte slot1, byte slot2)
        {
            // Returns true if the item swapped is a potion
            if (item is { ObjectType: 2594 or 2595 })
            { // HP/MP potion stack
                switch (item.ObjectType)
                {
                    case 2594:
                        if (!AddToPotionStack(2594))
                            return false;
                        break;
                    case 2595:
                        if (!AddToPotionStack(2595))
                            return false;
                        break;
                }

                SetItemNoLock(item2, slot1);
                SetItemNoLock(null, slot2);
                return true;
            }

            if (item2 is { ObjectType: 2594 or 2595 })
            { // HP/MP potion drop
                switch (item2.ObjectType)
                {
                    case 2594:
                        if (!RemovePotionStack(2594))
                            return false;
                        break;
                    case 2595:
                        if (!RemovePotionStack(2595))
                            return false;
                        break;
                }

                SetItemNoLock(item, slot2);
                SetItemNoLock(null, slot1);
                return true;
            }

            return false;
        }

        public bool AddToPotionStack(int itemId)
        { // Purely adds to the potion stack if there is room
            switch (itemId)
            {
                case 2594:
                    if (_healthPotionCount >= MAX_POTIONS)
                    {
                        Console.WriteLine("Too many potions!!");
                        return false;
                    }

                    Interlocked.Increment(ref _healthPotionCount);
                    _player.HealthPotions = _healthPotionCount;
                    return true;
                case 2595:
                    if (_magicPotionCount >= MAX_POTIONS)
                        return false;

                    Interlocked.Increment(ref _magicPotionCount);
                    _player.MagicPotions = _magicPotionCount;
                    return true;
            }

            return false;
        }

        public bool RemovePotionStack(int itemId)
        { // Purely removes from the potion stack if there is room
            switch (itemId)
            {
                case 2594:
                    if (_healthPotionCount <= 0)
                    { // We don't have any more potions
                        Console.WriteLine("Not enough potions!!");
                        return false;
                    }

                    Interlocked.Decrement(ref _healthPotionCount);
                    _player.HealthPotions = _healthPotionCount;
                    return true;
                case 2595:
                    if (_magicPotionCount <= 0)
                        return false;

                    Interlocked.Decrement(ref _magicPotionCount);
                    _player.MagicPotions = _magicPotionCount;
                    return true;
            }

            return false;
        }

        public override int GetNextAvailableSlot()
        {
            for (var i = 4; i < _invSize; i++)
            {
                if (_items[i] == null)
                    return i;
            }

            return -1;
        }

        public void Load(DbChar chr)
        {
            if (chr.ItemTypes == null || chr.ItemDatas == null)
                return;

            _healthPotionCount = _player.HealthPotions;
            _magicPotionCount = _player.MagicPotions;

            _invSize = chr.HasBackpack ? 20 : 12;
            SetItems(chr.ItemTypes, chr.ItemDatas);
        }

        public void Save(DbChar chr)
        {
            if (chr.ItemTypes == null || chr.ItemDatas == null)
                return;

            chr.ItemTypes = GetItemTypes();
            chr.ItemDatas = GetItemDatas();
        }

        public bool ApplyGemstones(byte slot, byte gemSlot, byte invSlot)
        {
            if (slot < 0 || slot >= _invSize || invSlot < 0 ||
                invSlot >= _invSize) // Check slots to be within valid values
                return false;

            var item = GetItemNoLock(slot);
            if (item.GemstoneLimit < 1 ||
                (item.Gemstones != null &&
                 item.Gemstones.Length >= item.GemstoneLimit)) // Can't put gemstones in this item
                return false;

            var newGemstones =
                item.Gemstones?.ToList() ?? new List<int>(); // Add new gemstone on top of the previous ones
            var gItem = GetItemNoLock(invSlot);
            if (gItem == null || gItem.Gemstone == null) // Shouldn't happen but just in case it isn't a gemstone
                return false;

            if (!gItem.Gemstone.SlotTypes.Contains(item.SlotType)) // Gemstone is not compatible with this item
                return false;

            newGemstones.Add(gItem.ObjectType);
            SetItemNoLock(null, invSlot);

            item.Gemstones = newGemstones.ToArray();

            UpdateSlots(slot, invSlot);

            return true;
        }

        public bool RemoveGemstones(byte slot, byte gemSlot, byte invSlot)
        {
            if (slot < 0 || slot >= _invSize || invSlot < 0 ||
                invSlot >= _invSize) // Check slots to be within valid values
                return false;

            var item = GetItemNoLock(slot);
            if (item.Gemstones == null || item.Gemstones.Length < 1) // Can't take any gemstones off of this item
                return false;

            if (gemSlot < 0 || gemSlot >= item.Gemstones.Length)
                return false;

            var gItemType = item.Gemstones[gemSlot]; // Get from item.Gemstones
            if (gItemType == -1)
                return false;

            if (GetItemNoLock(invSlot) != null) // Prevent overwriting existing items
                return false;

            var gItem = new Item(XmlLibrary.ItemDescs[(ushort)gItemType].Root); // Give item back into inventory
            SetItemNoLock(gItem, invSlot);

            var newGemstones = item.Gemstones.ToList();
            newGemstones.RemoveAt(gemSlot);

            item.Gemstones = newGemstones.ToArray();

            UpdateSlots(slot, invSlot);

            return true;
        }

        public bool SwapGemstones(byte slot, byte gemSlot, byte gemSlot2) // gemSlot2 is an inventory slot
        {
            if (slot < 0 || slot >= _invSize || gemSlot < 0 || gemSlot2 < 0 ||
                gemSlot2 >= _invSize) // Check slots to be within valid values
                return false;

            var item = GetItemNoLock(slot);
            if (item.Gemstones == null || item.Gemstones.Length < 1)
                return false;

            if (gemSlot < 0 || gemSlot >= item.Gemstones.Length)
                return false;

            var gItemType = item.Gemstones[gemSlot]; // Get from item.Gemstones
            if (gItemType == -1)
                return false;

            var invGem = GetItemNoLock(gemSlot2); // Tried to swap with a null item?
            if (invGem == null || invGem.Gemstone == null) // It's not a gemstone
                return false;

            if (!invGem.Gemstone.SlotTypes.Contains(item.SlotType)) // Gemstone is not compatible with this item
                return false;

            item.Gemstones[gemSlot] = invGem.ObjectType;
            item.ForceFieldUpdate((byte)ItemField.Gemstones);

            var gItem = new Item(XmlLibrary.ItemDescs[(ushort)gItemType].Root); // Give item back into inventory
            SetItemNoLock(gItem, gemSlot2);

            UpdateSlots(slot, gemSlot2);
            return true;
        }

        public static bool IsEquippable(Player player, Item item, byte slot)
        {
            if (item == null) return true;
            if (slot > 3) return true;

            var classDesc = XmlLibrary.PlayerDescs[player.Desc.ObjectType];
            return classDesc.SlotTypes[slot] == item.SlotType;
        }

        public int GetTotalFreeInventorySlots()
        {
            var count = 0;
            for (var i = 4; i < 12; i++)
                if (GetItem(i) == null)
                    count++;
            return count;
        }
    }
}