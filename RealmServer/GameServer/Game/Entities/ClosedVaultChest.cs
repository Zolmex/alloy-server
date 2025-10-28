#region

using Common;
using Common.Database;
using Common.Resources.Config;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Entities
{
    public class ClosedVaultChest : SellableObject
    {
        public ClosedVaultChest(ushort objType)
            : base(objType)
        {
            Price = NewAccountsConfig.Config.VaultSlotCost;
            Currency = CurrencyType.Fame;
        }

        public override string Purchase(Player plr)
        {
            if (World is not Vault vault)
                return "Not in Vault.";

            var acc = plr.User.Account;
            if (acc.Stats.Fame < Price)
                return "Not enough fame.";

            plr.AddCurrency(Currency, -Price);
            acc.VaultCount++;
            acc.Vault.AddVaultChest();
            DbClient.Save(acc);

            vault.OpenChest(this);
            return SUCCESS;
        }
    }
}