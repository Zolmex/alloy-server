#region

using Common;
using Common.Database;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;

#endregion

namespace GameServer.Game.Entities
{
    internal class Merchant : SellableObject
    {
        public bool Respawn { get; private set; }
        public int Stock { get => Stats.Get<int>(StatType.MerchandiseCount); set => Stats.Set(StatType.MerchandiseCount, value); }
        public int MinutesLeft { get => Stats.Get<int>(StatType.MerchandiseMinsLeft); set => Stats.Set(StatType.MerchandiseMinsLeft, value); }
        public int Discount { get => Stats.Get<int>(StatType.MerchandiseDiscount); set => Stats.Set(StatType.MerchandiseDiscount, value); }
        public int RankReq { get => Stats.Get<int>(StatType.MerchandiseRankReq); set => Stats.Set(StatType.MerchandiseRankReq, value); }

        private readonly MerchantDesc _merchDesc;
        private MerchandiseEntry _merchEntry;

        public Merchant(ushort objType, MerchantDesc desc) : base(objType)
        {
            _merchDesc = desc;
            Restock();
        }

        private void Restock()
        {
            _merchEntry = _merchDesc.Entries.RandomElement();

            Price = _merchEntry.Price;
            Currency = _merchEntry.Currency;
            RankReq = _merchEntry.RankReq;

            MerchandiseType = XmlLibrary.Id2Item(_merchEntry.ObjectId).ObjectType;
            Stock = Rand.Next(_merchEntry.MinStock, _merchEntry.MaxStock);
            MinutesLeft = Rand.Next(_merchEntry.MinDuration, _merchEntry.MaxDuration);

            Discount = 0;
            var roll = Rand.NextDouble();
            foreach (var discount in _merchEntry.Discounts)
                if (roll < discount.Probability && Discount < discount.Value)
                    Discount = discount.Value;

            if (Discount != 0)
                Price -= (int)(Price * (Discount / 100f));
        }

        public override void Initialize()
        {
            base.Initialize();

            AddMinuteCounter();
        }

        private void AddMinuteCounter()
        {
            RealmManager.AddTimedAction(MinutesLeft * 60000, () =>
            {
                if (Dead && !Respawn)
                    return;

                MinutesLeft--; // Keep counting the minutes if the merchant is supposed to respawn
                if (MinutesLeft <= 0)
                {
                    if (Respawn)
                    {
                        Respawn = false;
                        EnterWorld(World);
                    }

                    Restock();
                }
                else
                    AddMinuteCounter();
            });
        }

        public override string Purchase(Player plr)
        {
            var acc = plr.User.Account;
            if (!HasEnoughCapital(acc.Stats, Currency, Price))
                return $"Not enough {Currency}.";

            if (Stock <= 0)
                return "No stock";

            var slot = plr.Inventory.GetNextAvailableSlot();
            if (slot == -1)
                return "Not enough space in inventory.";

            var itemType = (ushort)MerchandiseType;
            plr.Inventory[slot] = new Item(XmlLibrary.ItemDescs[itemType].Root);
            plr.AddCurrency(Currency, -Price);

            Stock--;
            if (Stock <= 0) // If we ran out of stock, respawn when MinutesLeft is 0
            {
                Respawn = true;
                TryLeaveWorld();
            }

            return SUCCESS;
        }

        private static bool HasEnoughCapital(DbAccStats stats, CurrencyType currency, int amount)
        {
            return currency switch
            {
                CurrencyType.Gold => stats.Credits >= amount,
                CurrencyType.Fame => stats.Fame >= amount,
                CurrencyType.GuildFame => true,
                _ => false
            };
        }
    }
}