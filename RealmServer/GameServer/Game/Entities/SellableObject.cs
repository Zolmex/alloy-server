#region

using Common;

#endregion

namespace GameServer.Game.Entities
{
    public class SellableObject : Character
    {
        public const string SUCCESS = "Success!";

        public int MerchandiseType { get => Stats.Get<int>(StatType.MerchandiseType); set => Stats.Set(StatType.MerchandiseType, value); }
        public int Price { get => Stats.Get<int>(StatType.MerchandisePrice); set => Stats.Set(StatType.MerchandisePrice, value); }
        public CurrencyType Currency { get => (CurrencyType)Stats.Get<int>(StatType.MerchandiseCurrency); set => Stats.Set(StatType.MerchandiseCurrency, (int)value); }

        public SellableObject(ushort objType)
            : base(objType)
        { }

        public virtual string Purchase(Player plr)
        {
            return "Invalid merchant.";
        }
    }
}