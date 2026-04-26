using System.Collections.Generic;

namespace Common.Database.Models;

public class AccountLock : DbModel, IDbQueryable {
    public const string KEY_BASE = "accountLock";

    public AccountLock() {
        RegisterProperty("AccountId",
            (ref wtr) => wtr.Write(AccountId),
            (ref rdr) => AccountId = rdr.ReadInt32()
        );
        RegisterProperty("LockedId",
            (ref wtr) => wtr.Write(LockedId),
            (ref rdr) => LockedId = rdr.ReadInt32()
        );
    }

    public override string Key => KEY_BASE + $".{AccountId}.{LockedId}";

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public int LockedId { get; set; }

    public Account Locked { get; set; } = null!;

    public static IEnumerable<string> GetIncludes() {
        yield break;
    }

    public static AccountLock Read(string key) {
        var ret = new AccountLock();
        var split = key.Split('.');
        ret.AccountId = int.Parse(split[1]);
        ret.LockedId = int.Parse(split[2]);
        return ret;
    }

    public static string BuildKey(int accountId, int lockedId) {
        return KEY_BASE + $".{accountId}.{lockedId}";
    }
}