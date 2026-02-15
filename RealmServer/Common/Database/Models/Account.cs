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
        RegisterProperty("IsMuted",
            wtr => wtr.Write(IsMuted),
            rdr => IsMuted = rdr.ReadBoolean()
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
            wtr => wtr.Write(CreatedAt.ToUnixTimestamp()),
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
        RegisterProperty("Guild",
            wtr =>
            {
                var hasValue = Guild != null;
                wtr.Write(hasValue);
                if (hasValue)
                    Guild.WriteProperties(wtr);
            },
            rdr =>
            {
                Guild = DbModel.Read<Guild>(rdr);
                GuildId = Guild?.Id ?? 0;
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
                {
                    var skin = DbModel.Read<AccountSkin>(rdr);
                    if (skin != null)
                        AccountSkins.Add(skin);
                }
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
                {
                    var ign = DbModel.Read<AccountIgnore>(rdr);
                    if (ign != null)
                        AccountIgnores.Add(ign);
                }
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
                {
                    var locks = DbModel.Read<AccountLock>(rdr);
                    if (locks != null)
                        AccountLocks.Add(locks);
                }
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
                {
                    var chr = DbModel.Read<Character>(rdr);
                    if (chr != null)
                        Characters.Add(chr);
                }
            }
        );
        RegisterProperty("AccountVaults",
            wtr =>
            {
                wtr.Write((short)AccountVaults.Count);
                foreach (var vault in AccountVaults)
                {
                    var hasValue = vault != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        vault.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountVaults.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var vault = DbModel.Read<AccountVault>(rdr);
                    if (vault != null)
                        AccountVaults.Add(vault);
                }
            }
        );
        RegisterProperty("AccountGifts",
            wtr =>
            {
                wtr.Write((short)AccountGifts.Count);
                foreach (var gift in AccountGifts)
                {
                    var hasValue = gift != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        gift.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountGifts.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var gift = DbModel.Read<AccountGift>(rdr);
                    if (gift != null)
                        AccountGifts.Add(gift);
                }
            }
        );
        RegisterProperty("AccountMutes",
            wtr =>
            {
                wtr.Write((short)AccountMutes.Count);
                foreach (var mute in AccountMutes)
                {
                    var hasValue = mute != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        mute.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountMutes.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var mute = DbModel.Read<AccountMute>(rdr);
                    if (mute != null)
                        AccountMutes.Add(mute);
                }
            }
        );
        RegisterProperty("AccountBans",
            wtr =>
            {
                wtr.Write((short)AccountBans.Count);
                foreach (var bans in AccountBans)
                {
                    var hasValue = bans != null;
                    wtr.Write(hasValue);
                    if (hasValue)
                        bans.WriteProperties(wtr);
                }
            },
            rdr =>
            {
                AccountBans.Clear();
                var count = rdr.ReadInt16();
                for (var i = 0; i < count; i++)
                {
                    var ban = DbModel.Read<AccountBan>(rdr);
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