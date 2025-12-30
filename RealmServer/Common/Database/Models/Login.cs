using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public partial class Login
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
