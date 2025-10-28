#region

using Common.Database;
using GameServer.Game.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace GameServer.Game.Worlds
{
    public class Vault : World
    {
        public readonly int AccountId;

        private readonly List<Container> _openChests;
        private readonly List<Container> _openGiftChests;
        private List<Entity> _closedChests;
        private List<Entity> _closedGiftChests;
        private User _user;

        public Vault(User user)
            : base(VAULT, 0)
        {
            AccountId = user.Account.AccountId;

            _user = user;
            _user.GameInfo.Vault ??= this;

            _closedChests = new List<Entity>();
            _closedGiftChests = new List<Entity>();
            _openChests = new List<Container>();
            _openGiftChests = new List<Container>();
        }

        public override void Reset()
        {
            base.Reset();

            _closedChests.Clear();
            _closedGiftChests.Clear();
            _openChests.Clear();
            _openGiftChests.Clear();
        }

        protected override void InitializeEntities()
        {
            base.InitializeEntities();

            foreach (var kvp in Entities)
            {
                var en = kvp.Value;
                if (en is ClosedVaultChest)
                    _closedChests.Add(en);
                else if (en.Desc.Class == "ClosedGiftChest")
                    _closedGiftChests.Add(en);
            }

            if (_closedChests.Count == 0)
                return;

            _closedChests = OrderToCenter(_closedChests);
            _closedGiftChests = OrderToCenter(_closedGiftChests);

            for (var i = 0; i < _user.Account.VaultCount; i++)
            {
                var chest = OpenChest();
                chest?.LoadVaultChest(_user.Account.Vault, i);
            }

            var giftTypes = _user.Account.Gifts.ItemTypes;
            if (giftTypes.Count != 0)
            {
                using var stream = new MemoryStream(_user.Account.Gifts.ItemDatas.ToArray());
                using var dataReader = new BinaryReader(stream);

                OneWayContainer giftChest = null;
                for (var i = 0; i < giftTypes.Count; i++)
                {
                    var giftType = giftTypes[i];
                    if (giftType != -1) // Idk if it could happen but doesn't hurt to check
                    {
                        var slot = i % 8;
                        if (slot == 0) // Means we need a new chest
                            giftChest = OpenGiftChest();

                        giftChest.Inventory.SetItem(slot, giftType, dataReader);
                    }
                }
            }
        }

        public VaultChest OpenChest(Entity closedChest = null)
        {
            if (_closedChests.Count == 0)
                return null;

            closedChest ??= _closedChests[0]; // Remove chest closest to center
            closedChest.TryLeaveWorld();
            _closedChests.Remove(closedChest);

            var chest = new VaultChest(1284, _user.Account.AccountId);
            chest.Move(closedChest.Position.X, closedChest.Position.Y);
            chest.EnterWorld(this);
            _openChests.Add(chest);
            return chest;
        }

        public OneWayContainer OpenGiftChest(Entity closedGiftChest = null)
        {
            if (_closedGiftChests.Count == 0)
                return null;

            closedGiftChest ??= _closedGiftChests[0]; // Remove chest closest to center
            closedGiftChest.TryLeaveWorld();
            _closedGiftChests.Remove(closedGiftChest);

            var giftChest = new OneWayContainer(1860, _user.Account.AccountId);
            giftChest.Move(closedGiftChest.Position.X, closedGiftChest.Position.Y);
            giftChest.EnterWorld(this);
            _openGiftChests.Add(giftChest);
            return giftChest;
        }

        public override void AddEntity(Entity en)
        {
            base.AddEntity(en);

            if (en is Player plr && plr.AccountId == AccountId)
                _user = plr.User;
        }

        public override void RemoveEntity(Entity en)
        {
            base.RemoveEntity(en);

            if (!Deleted && en is OneWayContainer container && _openGiftChests.Contains(container)) // When a gift chest leaves we gotta spawn a closed gift chest
            {
                _openGiftChests.Remove(container);

                var closedGiftChest = Entity.Resolve(1859);
                closedGiftChest.Move(en.Position.X, en.Position.Y);
                closedGiftChest.EnterWorld(this);
                _closedGiftChests.Add(closedGiftChest);
                _closedGiftChests = OrderToCenter(_closedGiftChests);
            }

            if (_openChests.Count == 0 || en is not Player plr || plr.User.Account.AccountId != _user.Account.AccountId)
                return;

            // Player leaves
            for (var i = 0; i < _user.Account.VaultCount; i++) // If player bought chests the vault count and chests array will have already been updated
            {
                _user.Account.Vault.VaultChests[i].ItemTypes = _openChests[i].Inventory.GetItemTypes();
                _user.Account.Vault.VaultChests[i].ItemDatas = _openChests[i].Inventory.GetItemDatas();
            }

            // Save gifts
            _user.Account.Gifts.Clear();
            foreach (var giftChest in _openGiftChests)
                _user.Account.Gifts.AddGifts(giftChest.Inventory.GetItems());

            DbClient.Save(_user.Account.Vault, _user.Account.Gifts);
        }

        private static List<Entity> OrderToCenter(List<Entity> containers)
        {
            var minX = 0f;
            var minY = 0f;
            var maxX = 0f;
            var maxY = 0f;
            foreach (var container in containers) // Find the corner chests and find out the limits of the chest area
            {
                if (minX == 0 || container.Position.X < minX)
                    minX = container.Position.X;
                if (minY == 0 || container.Position.Y < minY)
                    minY = container.Position.Y;
                if (maxX == 0 || container.Position.X > maxX)
                    maxX = container.Position.X;
                if (maxY == 0 || container.Position.Y > maxY)
                    maxY = container.Position.Y;
            }

            var width = maxX - minX;
            var centerX = width / 2;

            return containers.OrderBy(c => c.DistSqr(minX + centerX, maxY)).ToList();
        }
    }
}