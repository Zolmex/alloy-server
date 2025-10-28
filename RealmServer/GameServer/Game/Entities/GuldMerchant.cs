#region

using Common;
using Common.Database;
using Common.Utilities;

#endregion

namespace GameServer.Game.Entities
{
    public class GuildMerchant : SellableObject
    {
        public GuildMerchant(ushort objType)
            : base(objType)
        {
            Price = Desc.XML.GetValue<int>("Price");
            Currency = CurrencyType.GuildFame;
        }

        public override string Purchase(Player plr)
        {
            if (World.DisplayName != "Guild Hall")
                return "Not in guild hall.";

            var acc = plr.User.Account;
            var guild = DbClient.GetGuild(acc.GuildId).SafeResult();
            if (guild == null)
                return "Not in guild.";

            if (acc.GuildRank < (int)GuildRank.Leader)
                return "Only leaders can upgrade the guild.";

            if (guild.Level == 3)
                return "Guild is already fully upgraded.";

            if (guild.Fame < Price)
                return "Not enough guild fame.";

            guild.Fame -= Price; // Charge accordingly
            guild.Level++;
            DbClient.Save(guild);

            RealmManager.ReloadGuildHall(guild.GuildId); // Delete old ghall and creates a new one

            return SUCCESS;
        }
    }
}