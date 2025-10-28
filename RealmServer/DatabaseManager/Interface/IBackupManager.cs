namespace DatabaseManager.Interface
{
    public interface IBackupManager
    {
        void Initialize(string baseDirectoryPath);
        Task BackupAsync(string backupName, bool isAuto, CancellationToken cancellationToken);
        Task RestoreByNameAsync(string backupName, bool backupCurrent, CancellationToken cancellationToken);
        Task RestoreBeforeDateTimeAsync(DateTime dateTime, bool backupCurrent, CancellationToken cancellationToken);

        Task RestoreBeforeTimeAgoAsync(TimeSpan time, bool backupCurrent, CancellationToken cancellationToken)
        {
            return RestoreBeforeDateTimeAsync(DateTime.UtcNow.Subtract(time), backupCurrent, cancellationToken);
        }

        bool BackupExists(string backupName);
        bool BackupExists(DateTime beforeDateTime);

        bool BackupExists(TimeSpan timeAgo)
        {
            return BackupExists(DateTime.UtcNow.Subtract(timeAgo));
        }
    }
}