using System;

namespace Common.Database.Models;

public class MuteRecord {
    public int Id { get; set; }
    public int TargetAccId { get; set; }
    public int ModeratorAccId { get; set; }
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}