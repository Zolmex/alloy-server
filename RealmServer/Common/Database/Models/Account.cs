using Common.Network;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Account
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
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short? Rank { get; set; }

    public string? GuildName { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsBanned { get; set; }

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
        wtr.Write((byte)1);
        
        wtr.Write(Id);
        wtr.Write(Name);
        wtr.Write(Rank ?? 0);
        wtr.Write(GuildName ?? "");
        wtr.Write(IsAdmin ?? false);
        wtr.Write(IsBanned ?? false);
        wtr.Write(MaxChars ?? (short)NewAccountsConfig.Config.MaxChars);
        wtr.Write(VaultCount ?? (short)NewAccountsConfig.Config.VaultCount);
        wtr.Write(NextCharId ?? 1);
        wtr.Write(CreatedAt!.Value.ToUnixTimestamp());
        wtr.Write(AccStatsId ?? 0);
        wtr.Write(LoginId ?? 0);
        wtr.Write(GuildMemberId ?? 0);
        
        if (AccStats != null)
            AccStats.Write(wtr);
        else wtr.Write((byte)0);
        if (GuildMember != null)
            GuildMember.Write(wtr);
        else wtr.Write((byte)0);
        if (Login != null)
            Login.Write(wtr);
        else wtr.Write((byte)0);
    }

    public static Account Read(NetworkReader rdr)
    {
        if (rdr.ReadByte() == 0) // Empty flag
            return null;
        
        var acc = new Account();
        acc.Id = rdr.ReadInt32();
        acc.Name = rdr.ReadUTF();
        acc.Rank = rdr.ReadInt16();
        acc.GuildName = rdr.ReadUTF();
        acc.IsAdmin = rdr.ReadBoolean();
        acc.IsBanned = rdr.ReadBoolean();
        acc.MaxChars = rdr.ReadInt16();
        acc.VaultCount = rdr.ReadInt16();
        acc.NextCharId = rdr.ReadInt16();
        acc.CreatedAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        acc.AccStatsId = rdr.ReadInt32();
        acc.LoginId = rdr.ReadInt32();
        acc.GuildMemberId = rdr.ReadInt32();
        acc.AccStats = AccountStat.Read(rdr);
        acc.GuildMember = GuildMember.Read(rdr);
        acc.Login = Login.Read(rdr);
        return acc;
    }
}
