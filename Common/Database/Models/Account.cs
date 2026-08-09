using System;
using System.Collections.Generic;
using Common.Resources.Config;

namespace Common.Database.Models;

public class Account {
    public static readonly Account Guest = new() {
        Id = -1,
        Name = "Guest",
        MaxChars = NewAccountsConfig.Config.MaxChars,
        VaultCount = NewAccountsConfig.Config.VaultCount,
        Stats = new AccountStats {
            CurrentCredits = NewAccountsConfig.Config.Credits,
            TotalCredits = NewAccountsConfig.Config.Credits, CurrentFame = NewAccountsConfig.Config.Fame,
            TotalFame = NewAccountsConfig.Config.Fame
        },
        CreatedAt = DateTime.Now
    };
    
    public int Id { get; set; }
    public Guid LockOwner { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
    public string GuildName { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
    public bool IsMuted { get; set; }
    public int MaxChars { get; set; }
    public int VaultCount { get; set; }
    public int NextCharId { get; set; }
    public DateTime CreatedAt { get; set; }
    public AccountStats Stats { get; set; }
    public AccountGifts Gifts { get; set; }
    public int GuildId { get; set; }
    public int GuildRank { get; set; }
    public int TotalGuildFame { get; set; }
    public DateTime LastSeenAt { get; set; }
    public List<int> OwnedSkins { get; set; } = [];
    public List<int> IgnoredAccounts { get; set; } = [];
    public List<int> LockedAccounts { get; set; } = [];
    public List<Character> Characters { get; set; } = [];
    public List<VaultChest> VaultChests { get; set; } = [];
}