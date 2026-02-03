using Common.Network;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Account : IDbModel
{
    public static readonly Account Guest = new()
    {
        Id = -1,
        Name = "Guest",
        Rank = 0,
        MaxChars = (short)NewAccountsConfig.Config.MaxChars,
        VaultCount = (short)NewAccountsConfig.Config.VaultCount,
        NextCharId = 1,
        AccStats = new AccountStat()
        {
            CurrentCredits = (uint)NewAccountsConfig.Config.Credits,
            TotalCredits = (uint)NewAccountsConfig.Config.Credits,
            CurrentFame = (uint)NewAccountsConfig.Config.Fame,
            TotalFame = (uint)NewAccountsConfig.Config.Fame
        },
        CreatedAt = DateTime.Now,
        IsAdmin = false
    };

    public string Key => $"account.{Id}";
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short? Rank { get; set; }

    public string? GuildName { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBanned { get; set; }

    public short? MaxChars { get; set; }

    public short? VaultCount { get; set; }

    public short? NextCharId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AccStatsId { get; set; }

    public int? LoginId { get; set; }

    public int? GuildMemberId { get; set; }

    public virtual AccountStat? AccStats { get; set; }

    public virtual ICollection<AccountSkin> AccountSkins { get; set; } = new List<AccountSkin>();

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual GuildMember? GuildMember { get; set; }

    public virtual Login? Login { get; set; }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(Rank ?? 0);
        wtr.Write(GuildName ?? "");
        wtr.Write(IsAdmin);
        wtr.Write(IsBanned);
        wtr.Write(MaxChars ?? (short)NewAccountsConfig.Config.MaxChars);
        wtr.Write(VaultCount ?? (short)NewAccountsConfig.Config.VaultCount);
        wtr.Write(NextCharId ?? 1);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
        if (AccStats != null)
            AccStats.Write(wtr);
        else wtr.Write(0);
        if (Login != null)
            Login.Write(wtr);
        else wtr.Write(0);
        if (GuildMember != null)
            GuildMember.Write(wtr);
        else wtr.Write(0);
    }

    public static Account Read(NetworkReader rdr)
    {
        var id = rdr.ReadInt32();
        if (id == 0) // ID flag. 0 for null
            return null;
        
        var acc = new Account();
        acc.Id = id;
        acc.Name = rdr.ReadUTF();
        acc.Rank = rdr.ReadInt16();
        acc.GuildName = rdr.ReadUTF();
        acc.IsAdmin = rdr.ReadBoolean();
        acc.IsBanned = rdr.ReadBoolean();
        acc.MaxChars = rdr.ReadInt16();
        acc.VaultCount = rdr.ReadInt16();
        acc.NextCharId = rdr.ReadInt16();
        acc.CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        acc.AccStats = AccountStat.Read(rdr);
        acc.AccStatsId = acc.AccStats?.Id ?? 0;
        acc.Login = Login.Read(rdr);
        acc.LoginId = acc.Login?.Id ?? 0;
        acc.GuildMember = GuildMember.Read(rdr);
        acc.GuildMemberId = acc.GuildMember?.Id ?? 0;
        return acc;
    }
}
