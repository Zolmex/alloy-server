using Common.Network;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class AccountVault : DbModel, IDbQueryable
{
    public const string KEY_BASE = "accountVault";
    
    public override string Key => KEY_BASE + $".{AccountId}.{SlotId}";
    
    public int AccountId { get; set; }

    public int SlotId { get; set; }

    public ushort ItemType { get; set; }

    public byte[]? ItemData { get; set; }

    public virtual Account Account { get; set; } = null!;

    public AccountVault()
    {
        RegisterProperty("AccountId",
            wtr => wtr.Write(AccountId),
            rdr => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("SlotId",
            wtr => wtr.Write(SlotId),
            rdr => SlotId = rdr.ReadInt32()
        );
        RegisterProperty("ItemType",
            wtr => wtr.Write(ItemType),
            rdr => ItemType = rdr.ReadUInt16()
        );
        RegisterProperty("ItemData",
            wtr => wtr.Write(ItemData ?? []),
            rdr => ItemData = rdr.Read<byte>()
        );
    }
    
    public static AccountVault Read(string key)
    {
        var ret = new AccountVault();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.SlotId = int.Parse(split[2]);
        return ret;
    }
    
    public static IEnumerable<string> GetIncludes()
    {
        yield break;
    }
    
    public static string BuildKey(int accountId, int slotId)
    {
        return KEY_BASE + $".{accountId}.{slotId}";
    }
}
