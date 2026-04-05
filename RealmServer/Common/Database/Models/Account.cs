using Common.Network;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Database.Models;

public partial class Account : DbModel, IDbQueryable
{
    public const string KEY_BASE = "account";
    
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

    public override string Key => KEY_BASE + $".{Id}";

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short Rank { get; set; }

    public string? GuildName { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBanned { get; set; }
    
    public bool IsMuted { get; set; }

    public short MaxChars { get; set; }

    public short VaultCount { get; set; }

    public short NextCharId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int AccStatsId { get; set; }

    public int LoginId { get; set; }

    public int? GuildMemberId { get; set; }
    
    public int? GuildId { get; set; }

    public virtual AccountStat AccStats { get; set; }

    public virtual ICollection<AccountSkin> AccountSkins { get; set; } = new List<AccountSkin>();

    public virtual ICollection<AccountIgnore> AccountIgnores { get; set; } = new List<AccountIgnore>();

    public virtual ICollection<AccountLock> AccountLocks { get; set; } = new List<AccountLock>();

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<AccountVault> AccountVaults { get; set; } = new List<AccountVault>();
    
    public virtual ICollection<AccountGift> AccountGifts { get; set; } = new List<AccountGift>();

    public virtual ICollection<AccountMute> AccountMutes { get; set; } = new List<AccountMute>();
    
    public virtual ICollection<AccountBan> AccountBans { get; set; } = new List<AccountBan>();
    
    public virtual GuildMember? GuildMember { get; set; }
    
    public virtual Guild? Guild { get; set; }

    public virtual Login Login { get; set; }

    public Account()
    {
        RegisterProperty("Id",
           (ref wtr) => wtr.Write(Id),
            (ref rdr) => Id = rdr.ReadInt32()
        );
        RegisterProperty("Name",
           (ref wtr) => wtr.WriteUTF(Name),
            (ref rdr) => Name = rdr.ReadUTF()
        );
        RegisterProperty("Rank",
           (ref wtr) => wtr.Write(Rank),
            (ref rdr) => Rank = rdr.ReadInt16()
        );
        RegisterProperty("GuildName",
           (ref wtr) => wtr.WriteUTF(GuildName ?? ""),
            (ref rdr) => GuildName = rdr.ReadUTF()
        );
        RegisterProperty("IsAdmin",
           (ref wtr) => wtr.Write(IsAdmin),
            (ref rdr) => IsAdmin = rdr.ReadBoolean()
        );
        RegisterProperty("IsBanned",
           (ref wtr) => wtr.Write(IsBanned),
            (ref rdr) => IsBanned = rdr.ReadBoolean()
        );
        RegisterProperty("IsMuted",
           (ref wtr) => wtr.Write(IsMuted),
            (ref rdr) => IsMuted = rdr.ReadBoolean()
        );
        RegisterProperty("MaxChars",
           (ref wtr) => wtr.Write(MaxChars),
            (ref rdr) => MaxChars = rdr.ReadInt16()
        );
        RegisterProperty("VaultCount",
           (ref wtr) => wtr.Write(VaultCount),
            (ref rdr) => VaultCount = rdr.ReadInt16()
        );
        RegisterProperty("NextCharId",
           (ref wtr) => wtr.Write(NextCharId),
            (ref rdr) => NextCharId = rdr.ReadInt16()
        );
        RegisterProperty("CreatedAt",
           (ref wtr) => wtr.Write(CreatedAt.ToUnixTimestamp()),
            (ref rdr) => CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32())
        );
        RegisterProperty("AccStats",
            (ref wtr) =>
            {
                var hasValue = AccStats != null;
                wtr.Write(hasValue);
                if (hasValue)
                    AccStats.WriteProperties(ref wtr);
            },
            (ref rdr) =>
            {
                AccStats = AccountStat.Read(ref rdr);
                AccStatsId = AccStats?.Id ?? 0;
            }
        );
        RegisterProperty("Login",
            (ref wtr) =>
            {
                var hasValue = Login != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Login.WriteProperties(ref wtr);
            },
            (ref rdr) =>
            {
                Login = DbModel.Read<Login>(ref rdr);
                LoginId = Login?.Id ?? 0;
            }
        );
        RegisterProperty("GuildMember",
            (ref wtr) =>
            {
                var hasValue = GuildMember != null;
                wtr.Write(hasValue);
                if (hasValue)
                    GuildMember.WriteProperties(ref wtr);
            },
            (ref rdr) =>
            {
                GuildMember = DbModel.Read<GuildMember>(ref rdr);
                GuildMemberId = GuildMember?.Id ?? 0;
            }
        );
        RegisterProperty("Guild",
            (ref wtr) =>
            {
                var hasValue = Guild != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Guild.WriteProperties(ref wtr);
            },
            (ref rdr) =>
            {
                Guild = DbModel.Read<Guild>(ref rdr);
                GuildId = Guild?.Id ?? 0;
            }
        );
        RegisterProperty("AccountSkins",
            (ref wtr) =>
            {
                wtr.Write((short)AccountSkins.Count);
                foreach (var skin in AccountSkins)
                {
                    var hasValue = skin != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        skin.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountSkins.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var skin = DbModel.Read<AccountSkin>(ref rdr);
                    if (skin != null)
                        AccountSkins.Add(skin);
                }
            }
        );
        RegisterProperty("AccountIgnores",
            (ref wtr) =>
            {
                wtr.Write((short)AccountIgnores.Count);
                foreach (var ignored in AccountIgnores)
                {
                    var hasValue = ignored != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        ignored.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountIgnores.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var ign = DbModel.Read<AccountIgnore>(ref rdr);
                    if (ign != null)
                        AccountIgnores.Add(ign);
                }
            }
        );
        RegisterProperty("AccountLocks",
            (ref wtr) =>
            {
                wtr.Write((short)AccountLocks.Count);
                foreach (var locked in AccountLocks)
                {
                    var hasValue = locked != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        locked.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountLocks.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var locks = DbModel.Read<AccountLock>(ref rdr);
                    if (locks != null)
                        AccountLocks.Add(locks);
                }
            }
        );
        RegisterProperty("Characters",
            (ref wtr) =>
            {
                wtr.Write((short)Characters.Count);
                foreach (var chr in Characters)
                {
                    var hasValue = chr != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        chr.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                Characters.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var chr = DbModel.Read<Character>(ref rdr);
                    if (chr != null)
                        Characters.Add(chr);
                }
            }
        );
        RegisterProperty("AccountVaults",
            (ref wtr) =>
            {
                wtr.Write((short)AccountVaults.Count);
                foreach (var vault in AccountVaults)
                {
                    var hasValue = vault != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        vault.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountVaults.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var vault = DbModel.Read<AccountVault>(ref rdr);
                    if (vault != null)
                        AccountVaults.Add(vault);
                }
            }
        );
        RegisterProperty("AccountGifts",
            (ref wtr) =>
            {
                wtr.Write((short)AccountGifts.Count);
                foreach (var gift in AccountGifts)
                {
                    var hasValue = gift != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        gift.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountGifts.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var gift = DbModel.Read<AccountGift>(ref rdr);
                    if (gift != null)
                        AccountGifts.Add(gift);
                }
            }
        );
        RegisterProperty("AccountMutes",
            (ref wtr) =>
            {
                wtr.Write((short)AccountMutes.Count);
                foreach (var mute in AccountMutes)
                {
                    var hasValue = mute != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        mute.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountMutes.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var mute = DbModel.Read<AccountMute>(ref rdr);
                    if (mute != null)
                        AccountMutes.Add(mute);
                }
            }
        );
        RegisterProperty("AccountBans",
            (ref wtr) =>
            {
                wtr.Write((short)AccountBans.Count);
                foreach (var bans in AccountBans)
                {
                    var hasValue = bans != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        bans.WriteProperties(ref wtr);
                }
            },
            (ref rdr) =>
            {
                AccountBans.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var ban = DbModel.Read<AccountBan>(ref rdr);
                    if (ban != null)
                        AccountBans.Add(ban);
                }
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
    
    public static string BuildKey(int accountId)
    {
        return KEY_BASE + $".{accountId}";
    }
}