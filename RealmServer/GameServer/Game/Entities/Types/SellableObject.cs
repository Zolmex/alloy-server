#region

using Common;

#endregion

namespace GameServer.Game.Entities.Types;

public class SellableObject : CharacterEntity {
    public const string SUCCESS = "Success!";

    public SellableObject(ushort objType)
        : base(objType) { }

    public int MerchandiseType {
        get => Stats.GetInt(StatType.MerchandiseType);
        set => Stats.Set(StatType.MerchandiseType, value);
    }

    public int Price {
        get => Stats.GetInt(StatType.MerchandisePrice);
        set => Stats.Set(StatType.MerchandisePrice, value);
    }

    public CurrencyType Currency {
        get => (CurrencyType)Stats.GetInt(StatType.MerchandiseCurrency);
        set => Stats.Set(StatType.MerchandiseCurrency, (int)value);
    }

    public virtual string Purchase(Player plr) {
        return "Invalid merchant.";
    }
}