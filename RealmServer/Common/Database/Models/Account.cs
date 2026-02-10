using Common.Network;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Database.Models;

public partial class Account : DbModel, IDbQueryable
{
    public static readonly Account Guest = new()
    {
        Id = -1,
        Name = "Guest",
        MaxChars = (short)NewAccountsConfig.Config.MaxChars,
        VaultCount = (short)NewAccountsConfig.Config.VaultCount,
        NextCharId = 1,
        AccStats = new AccountStat() { CurrentCredits = (uint)NewAccountsConfig.Config.Credits, TotalCredits = (uint)NewAccountsConfig.Config.Credits, CurrentFame = (uint)NewAccountsConfig.Config.Fame, TotalFame = (uint)NewAccountsConfig.Config.Fame },
        CreatedAt = DateTime.Now,
    };

    public override string Key => $"account.{Id}";

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short Rank { get; set; }

    public string? GuildName { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBanned { get; set; }

    public short MaxChars { get; set; }

    public short VaultCount { get; set; }

    public short NextCharId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int AccStatsId { get; set; }

    public int LoginId { get; set; }

    public int? GuildMemberId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    public virtual ICollection<AccountSkin> AccountSkins { get; set; } = new List<AccountSkin>();

    public virtual ICollection<AccountIgnore> AccountIgnores { get; set; } = new List<AccountIgnore>();

    public virtual ICollection<AccountLock> AccountLocks { get; set; } = new List<AccountLock>();

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual GuildMember? GuildMember { get; set; }

    public virtual Login? Login { get; set; }

    protected override void Prepare()
    {
        RegisterProperty("Id",
            wtr => wtr.Write(Id),
            rdr => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
            wtr => wtr.Write(Name),
            rdr => Name = rdr.ReadUTF()
        );
        RegisterProperty("Rank",
            wtr => wtr.Write(Rank),
            rdr => Rank = rdr.ReadInt16()
        );
        RegisterProperty("GuildName",
            wtr => wtr.Write(GuildName ?? ""),
            rdr => GuildName = rdr.ReadUTF()
        );
        RegisterProperty("IsAdmin",
            wtr => wtr.Write(IsAdmin),
            rdr => IsAdmin = rdr.ReadBoolean()
        );
        RegisterProperty("IsBanned",
            wtr => wtr.Write(IsBanned),
            rdr => IsBanned = rdr.ReadBoolean()
        );
        RegisterProperty("MaxChars",
            wtr => wtr.Write(MaxChars),
            rdr => MaxChars = rdr.ReadInt16()
        );
        RegisterProperty("VaultCount",
            wtr => wtr.Write(VaultCount),
            rdr => VaultCount = rdr.ReadInt16()
        );
        RegisterProperty("NextCharId",
            wtr => wtr.Write(NextCharId),
            rdr => NextCharId = rdr.ReadInt16()
        );
        RegisterProperty("CreatedAt",
            wtr => wtr.Write(CreatedAt!.Value.ToUnixTimestamp()),
            rdr => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("AccStats",
            wtr =>
            {
                var hasValue = AccStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    AccStats.WriteProperties(wtr);
            },
            rdr =>
            {
                AccStats = AccountStat.Read(rdr);
                AccStatsId = AccStats?.Id ?? 0;
            }
        );
        RegisterProperty("Login",
            wtr =>
            {
                var hasValue = Login != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Login.WriteProperties(wtr);
            },
            rdr =>
            {
                Login = DbModel.Read<Login>(rdr);
                LoginId = Login?.Id ?? 0;
            }
        );
        RegisterProperty("GuildMember",
            wtr =>
            {
                var hasValue = GuildMember != null;
                wtr.Write(hasValue);
                if (hasValue)
                    GuildMember.WriteProperties(wtr);
            },
            rdr =>
            {
                GuildMember = DbModel.Read<GuildMember>(rdr);
                GuildMemberId = GuildMember?.Id ?? 0;
            }
        );
        RegisterProperty("AccountSkins",
            wtr =>
            {
                wtr.Write((short)AccountSkins.Count);
                foreach (var skin in AccountSkins)
                {
                    var hasValue = skin != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        skin.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountSkins.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                    AccountSkins.Add(DbModel.Read<AccountSkin>(rdr));
            }
        );
        RegisterProperty("AccountIgnores",
            wtr =>
            {
                wtr.Write((short)AccountIgnores.Count);
                foreach (var ignored in AccountIgnores)
                {
                    var hasValue = ignored != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        ignored.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountIgnores.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                    AccountIgnores.Add(DbModel.Read<AccountIgnore>(rdr));
            }
        );
        RegisterProperty("AccountLocks",
            wtr =>
            {
                wtr.Write((short)AccountLocks.Count);
                foreach (var locked in AccountLocks)
                {
                    var hasValue = locked != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        locked.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountLocks.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                    AccountLocks.Add(DbModel.Read<AccountLock>(rdr));
            }
        );
        RegisterProperty("Characters",
            wtr =>
            {
                wtr.Write((short)Characters.Count);
                foreach (var chr in Characters)
                {
                    var hasValue = chr != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        chr.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                Characters.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                    Characters.Add(DbModel.Read<Character>(rdr));
            }
        );
    }

    public static Account Read(string key)
    {
        var ret = new Account();
        var split = key.Split('.');
        ret.Id = int.Parse(split[1]);
        return ret;
    }
    
    public static IEnumerable<string> GetIncludes()
    {
        yield return "AccStats.ClassStats";
        yield return "Login";
        yield return "GuildMember";
        yield return "AccountSkins";
        yield return "AccountIgnores";
        yield return "AccountLocks";
        yield return "Characters.CharStats";
        yield return "Characters.CombatStats";
        yield return "Characters.DungeonStats";
        yield return "Characters.ExploStats";
        yield return "Characters.KillStats";
        yield return "Characters.CharacterInventories";
    }
}