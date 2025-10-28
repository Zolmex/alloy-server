#region

using Common.Control;
using Common.Control.Message;
using Common.Resources.Config;
using Common.Utilities;
using DatabaseManager.Interface;
using Microsoft.Extensions.Hosting;

#endregion

namespace DatabaseManager.Service
{
    public class BackupService : BackgroundService
    {
        private readonly Logger _logger = new(typeof(BackupService));
        private readonly DatabaseManagerConfig _config = DatabaseManagerConfig.Config;
        private CancellationToken _cancellationToken;

        private bool _restoreRunning;

        #region Dependency Injection

        private readonly IBackupManager _backupManager;
        private readonly IRedisConnection _redisConnection;

        public BackupService(
            IBackupManager backupManager,
            IRedisConnection redisConnection)
        {
            _backupManager = backupManager;
            _redisConnection = redisConnection;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Starting.");
            _cancellationToken = stoppingToken;

            try
            {
                await _redisConnection.ConnectAsync();
            }
            catch (Exception e)
            {
                _logger.Error($"Could not connect to Redis Server: {e}");
                Shutdown();
                return;
            }

            _logger.Info("Connected to Redis Server!");

            var info = await _redisConnection.GetRdbFilePathAsync();

            var redisFolderPath = Path.GetDirectoryName(info);
            if (redisFolderPath is null)
            {
                _logger.Error("Could not get Redis folder path.");
                Shutdown();
                return;
            }

            _backupManager.Initialize(redisFolderPath);

            ServerControl.Connect(MemberType.DatabaseManager, "Database Manager");

            ServerControl.Subscribe<string>(ControlChannel.DbBackup, OnBackupCommand);
            ServerControl.Subscribe<RestoreCommandInfo>(ControlChannel.DbRestore, OnRestoreCommand);
            ServerControl.Subscribe<bool>(ControlChannel.DbWipe, OnWipeCommand);

            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_config.AutoSaveInterval, _cancellationToken);

                if (_restoreRunning) continue;

                if (!_redisConnection.IsConnected)
                {
                    _logger.Error("Redis connection lost. Stopping application.");
                    Shutdown();
                    return;
                }

                await BackupAsync(DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss"), true, _cancellationToken);
            }
        }

        private void Shutdown()
        {
            Environment.Exit(0);
        }

        private async void OnWipeCommand(object? sender, ControlMessage<bool> message)
        {
            if (_restoreRunning) return;

            // Message Content for this channel is the direction of the wipe.
            // True = Message when wipe happened.
            // False = Message to trigger a wipe.
            if (message.Content)
            {
                return;
            }

            _restoreRunning = true;
            //await ShutdownServer(TimeSpan.Zero, "Database wipe.", false);
            await _redisConnection.WipeAsync();
            _restoreRunning = false;

            ServerControl.Publish(ControlChannel.DbWipe, ServerControl.Host.InstanceID, null, true);
            _logger.Info("Database wiped.");
        }

        private async void OnBackupCommand(object? sender, ControlMessage<string> message)
        {
            if (_restoreRunning) return;

            var name = string.IsNullOrEmpty(message.Content) ? DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss") : message.Content;
            await BackupAsync(name, false, _cancellationToken);
        }

        private async void OnRestoreCommand(object? sender, ControlMessage<RestoreCommandInfo> message)
        {
            if (_restoreRunning) return;

            if (message.Content.TimeAgo is not null)
            {
                if (!_backupManager.BackupExists((TimeSpan)message.Content.TimeAgo))
                {
                    _logger.Error("No backup exists before the specified time.");
                    return;
                }

                _restoreRunning = true;
                _logger.Debug($"Restore before TimeAgo command called. Restoring latest backup before '{message.Content.TimeAgo}' ago.");
                await RestoreAsync((TimeSpan)message.Content.TimeAgo, message.Content.ShutdownDelay);
                _restoreRunning = false;
                return;
            }

            if (message.Content.DateTime is not null)
            {
                if (!_backupManager.BackupExists((DateTime)message.Content.DateTime))
                {
                    _logger.Error("No backup exists before the specified time.");
                    return;
                }

                _restoreRunning = true;
                _logger.Debug($"Restore before DateTime command called. Restoring latest backup before '{message.Content.DateTime}'.");
                await RestoreAsync((DateTime)message.Content.DateTime, message.Content.ShutdownDelay);
                _restoreRunning = false;
                return;
            }

            if (message.Content.Name is not null)
            {
                if (!_backupManager.BackupExists(message.Content.Name))
                {
                    _logger.Error("No backup exists with the specified name.");
                    return;
                }

                _restoreRunning = true;
                _logger.Debug($"Restore by name command called. Restoring '{message.Content.Name}'.");
                await RestoreAsync(message.Content.Name, message.Content.ShutdownDelay);
                _restoreRunning = false;
                return;
            }

            _logger.Error("Failed to restore: Invalid backup info.");
        }

        private async Task BackupAsync(string name, bool isAuto, CancellationToken cancellationToken)
        {
            if (_restoreRunning) return;

            await _redisConnection.SaveAsync();
            var initialLastSaved = await _redisConnection.GetLastSaveAsync();
            await WaitForLastSavedChange(initialLastSaved, cancellationToken);
            await _backupManager.BackupAsync(name, isAuto, _cancellationToken);
        }

        private async Task RestoreAsync(string backupName, TimeSpan shutdownIn, bool backupCurrent = true)
        {
            await ShutdownServer(shutdownIn, "Database backup restore.");
            await _backupManager.RestoreByNameAsync(backupName, backupCurrent, _cancellationToken);
        }

        private async Task RestoreAsync(DateTime dateTime, TimeSpan shutdownIn, bool backupCurrent = true)
        {
            await ShutdownServer(shutdownIn, "Database backup restore.");
            await _backupManager.RestoreBeforeDateTimeAsync(dateTime, backupCurrent, _cancellationToken);
        }

        private async Task RestoreAsync(TimeSpan time, TimeSpan shutdownIn, bool backupCurrent = true)
        {
            await ShutdownServer(shutdownIn, "Database backup restore.");
            await _backupManager.RestoreBeforeTimeAgoAsync(time, backupCurrent, _cancellationToken);
        }

        private async Task ShutdownServer(TimeSpan shutdownIn, string reason, bool closeRedis = true)
        {
            _logger.Debug("Backup restore requested, shutdown message sent.");

            var shutdownInfo = new ShutdownInfo() { ShutdownDelay = shutdownIn, Reason = reason };

            ServerControl.Publish(ControlChannel.Shutdown, ServerControl.Host.InstanceID, null, shutdownInfo);
            _logger.Debug("Shutdown message sent.");

            await Task.Delay(shutdownIn, _cancellationToken);

            if (closeRedis) _redisConnection.Disconnect();

            // Wait for a bit to make sure everything is closed.
            await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        }

        private async Task WaitForLastSavedChange(DateTime initialLastSaved, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var lastSaved = await _redisConnection.GetLastSaveAsync();

                // Value has changed, we can continue.
                if (lastSaved != DateTime.MinValue && lastSaved > initialLastSaved)
                    return;

                await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
            }
        }
    }
}