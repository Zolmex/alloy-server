#region

using Common.Resources.Config;
using Common.Utilities;
using DatabaseManager.Data;
using DatabaseManager.Interface;
using Newtonsoft.Json;

#endregion

namespace DatabaseManager.Implementation
{
    public class FileBackupManager : IBackupManager
    {
        private static readonly ILogger _logger = new Logger(typeof(FileBackupManager));
        private readonly DatabaseManagerConfig _config = DatabaseManagerConfig.Config;

        private string _baseDirectoryPath = string.Empty;
        private string _rdbPath = string.Empty;
        private string _backupFolderPath = string.Empty;
        private string _backupDataPath = string.Empty;

        private Dictionary<string, BackupInfo> _backupData = new();

        private string GetBackupRdbPath(string backupName)
        {
            return Path.Combine(_backupFolderPath, $"{backupName}.rdb");
        }

        public void Initialize(string baseDirectoryPath)
        {
            _rdbPath = Path.Combine(baseDirectoryPath, "dump.rdb");
            _backupFolderPath = Path.Combine(baseDirectoryPath, "Backup");

            if (!Directory.Exists(_backupFolderPath))
            {
                Directory.CreateDirectory(_backupFolderPath);
            }

            _backupDataPath = Path.Combine(_backupFolderPath, "backupData.json");

            if (File.Exists(_backupDataPath))
            {
                _backupData =
                    JsonConvert.DeserializeObject<Dictionary<string, BackupInfo>>(File.ReadAllText(_backupDataPath)) ??
                    new Dictionary<string, BackupInfo>();

                foreach (var backupInfo in _backupData.Values)
                {
                    if (!File.Exists(backupInfo.RdbPath))
                        _backupData.Remove(backupInfo.Name);
                }
            }
        }

        public async Task BackupAsync(string backupName, bool isAuto = false, CancellationToken cancellationToken = default)
        {
            if (_backupData.Count >= _config.AutoBackupLimit)
            {
                var oldestAutoBackup = _backupData.Values
                    .Where(b => b.IsAuto)
                    .MinBy(b => b.SavedAt);

                await Task.Run(() =>
                {
                    try
                    {
                        File.Delete(oldestAutoBackup.RdbPath);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Failed to Delete backup file: {e}");
                        return;
                    }

                    _backupData.Remove(oldestAutoBackup.Name);
                }, cancellationToken);
            }

            try
            {
                await FileUtils.CopyAsync(_rdbPath, GetBackupRdbPath(backupName), cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to BackupAsync: CopyAsync Failed. {e}");
                return;
            }

            await SaveBackupInfoAsync(backupName, isAuto, cancellationToken);

            _logger.Info($"Backup saved at {DateTime.UtcNow:G}.");
        }

        public async Task RestoreByNameAsync(string backupName, bool backupCurrent = true, CancellationToken cancellationToken = default)
        {
            if (!TryGetBackupInfoByName(backupName, out var data) || data is null)
            {
                _logger.Error($"Failed to RestoreAsync: Backup '{backupName}' does not exist.");
                return;
            }

            if (backupCurrent)
            {
                await BackupAsync(DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss"), true, cancellationToken);
            }

            try
            {
                await FileUtils.CopyAsync(data.RdbPath, _rdbPath, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to CopyAsync: {e}");
                return;
            }

            _logger.Info($"Backup '{backupName}' restored.");
        }

        public async Task RestoreBeforeDateTimeAsync(DateTime dateTime, bool backupCurrent, CancellationToken cancellationToken)
        {
            if (!TryGetBackupInfoBeforeDateTime(dateTime, out var data) || data is null)
            {
                _logger.Error($"Failed to RestoreAsync: Could not find backup before {dateTime:G}.");
                return;
            }

            if (backupCurrent)
            {
                await BackupAsync(DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss"), true, cancellationToken);
            }

            try
            {
                await FileUtils.CopyAsync(data.RdbPath, _rdbPath, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to CopyAsync: {e}");
                return;
            }

            _logger.Info($"Backup '{data.Name}' restored.");
        }

        private async Task SaveBackupInfoAsync(string backupName, bool isAuto = false, CancellationToken cancellationToken = default)
        {
            var backupInfo = new BackupInfo() { Name = backupName, RdbPath = GetBackupRdbPath(backupName), SavedAt = DateTime.UtcNow, IsAuto = isAuto };

            if (_backupData.TryAdd(backupName, backupInfo))
            {
                await File.WriteAllTextAsync(_backupDataPath, JsonConvert.SerializeObject(_backupData), cancellationToken);
            }
        }

        private bool TryGetBackupInfoByName(string backupName, out BackupInfo? data)
        {
            return _backupData.TryGetValue(backupName, out data);
        }

        private bool TryGetBackupInfoBeforeDateTime(DateTime backupTime, out BackupInfo? data)
        {
            try
            {
                data = _backupData.Values
                    .Where(d => d.SavedAt <= backupTime)
                    .OrderBy(d => d.SavedAt)
                    .Last();
            }
            catch (Exception e)
            {
                _logger.Error($"Could not find backup before {backupTime:G}: {e}");
                data = null;
                return false;
            }

            return true;
        }

        public bool BackupExists(string backupName)
        {
            return _backupData.ContainsKey(backupName);
        }

        public bool BackupExists(DateTime beforeDateTime)
        {
            return _backupData.Values.Any(d => d.SavedAt <= beforeDateTime);
        }
    }
}