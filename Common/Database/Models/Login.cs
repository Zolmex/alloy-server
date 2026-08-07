using System;

namespace Common.Database.Models;

public class Login {
    public int Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string IpAddress { get; set; }
}