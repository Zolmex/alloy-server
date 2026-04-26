using System.Collections.Generic;

namespace Common.Database.Models;

public class AccountSkin : DbModel, IDbQueryable {
    public const string KEY_BASE = "accountSkin";

    public AccountSkin() {
        RegisterProperty("AccountId",
            (ref wtr) => wtr.Write(AccountId),
            (ref rdr) => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("SkinType",
            (ref wtr) => wtr.Write(SkinType),
            (ref rdr) => SkinType = rdr.ReadInt32()
        );
    }

    public override string Key => KEY_BASE + $".{AccountId}.{SkinType}";

    public int AccountId { get; set; }

    public int SkinType { get; set; }

    public virtual Account Account { get; set; } = null!;

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static AccountSkin Read(string key) {
        var ret = new AccountSkin();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.SkinType = int.Parse(split[2]);
        return ret;
    }

    public static string BuildKey(int accountId, int skinType) {
        return KEY_BASE + $".{accountId}.{skinType}";
    }
}