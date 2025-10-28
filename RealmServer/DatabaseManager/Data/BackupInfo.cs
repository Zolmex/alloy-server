namespace DatabaseManager.Data
{
    public class BackupInfo
    {
        public string Name { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        public bool IsAuto { get; set; } = false;
        public string RdbPath { get; set; } = string.Empty;
    }
}