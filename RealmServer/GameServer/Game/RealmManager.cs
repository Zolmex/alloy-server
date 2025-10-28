#region

using Common.Database;
using Common.Resources.Config;
using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.Chat.Commands;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network.API;
using GameServer.Game.Worlds;
using GameServer.Utilities.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace GameServer.Game
{
    public struct RealmTime
    {
        public float TickCountDecimal;
        public long TickCount;
        public long TotalElapsedMs;
        public int ElapsedMsDelta;
    }

    public static class RealmManager
    {
        private static readonly Logger _log = new(typeof(RealmManager));

        public static Nexus NexusInstance { get; private set; }

        public static Stopwatch RealTime; // Used to know the REAL elapsed time in the server accross all threads
        public static RealmTime WorldTime; // This field is updated every tick
        public static int TPS;

        public static readonly LazyCollection<User> Users = new();
        public static readonly ConcurrentDictionary<User, int> UserAccIds = new();
        public static readonly ConcurrentDictionary<int, World> Worlds = new();

        public static readonly List<Tuple<long, Action>> Timers = new();

        public static readonly ConcurrentDictionary<int, string> ActiveRealms = new();
        private static readonly Dictionary<int, World> _guildHalls = new();

        private static readonly object _updateLock = new();
        private static event Action _onUpdate;
        private static event Action<User> _userUpdate;
        private static APIEventManager apiEventManager;

        public static void Init()
        {
            TPS = GameServerConfig.Config.TPS;
            Users.OnAdd += OnUserAdded;

            CommandManager.Load();

            DbClient.UpdateLegends(); // Update legends every day
            AddTimedAction((int)TimeSpan.FromDays(1).TotalMilliseconds, DbClient.UpdateLegends);

            apiEventManager = new APIEventManager();

            NexusInstance = new Nexus();
            AddWorld(NexusInstance);

            _log.Info("RealmManager initialized.");
        }

        public static void Run(int mspt)
        {
            var lagMs = (int)(mspt * 1.5);
            var sw = Stopwatch.StartNew();
            RealTime = Stopwatch.StartNew();
            while (true)
            {
                Update();

                // This approach uses more CPU power but it's much more accurate than others (e.g Thread.Sleep, ManualResetEvent)
                if (sw.ElapsedMilliseconds < mspt)
                    continue;

                WorldTime.ElapsedMsDelta = (int)sw.ElapsedMilliseconds;
                WorldTime.TotalElapsedMs += sw.ElapsedMilliseconds;
                WorldTime.TickCountDecimal += WorldTime.ElapsedMsDelta / (float)mspt;
                if (WorldTime.TickCountDecimal > 1)
                {
                    var ticks = (int)WorldTime.TickCountDecimal;
                    WorldTime.TickCountDecimal -= ticks;
                    WorldTime.TickCount += ticks;
                }

                if (WorldTime.ElapsedMsDelta >= lagMs)
                    _log.Warn($"LAGGED | MsPT: {mspt} Elapsed: {WorldTime.ElapsedMsDelta}");

                sw.Restart();

                HandleTimers();

                Parallel.ForEach(Worlds.Values, w => w.Tick(WorldTime));

                DbClient.SetPlayerCount(Users.Count);
            }
        }

        private static void Update()
        {
            using (TimedLock.Lock(_updateLock))
            {
                _onUpdate?.Invoke();
                _onUpdate = null;
            }

            foreach (var world in Worlds.Values)
                world.Update();

            Users.Update();
            Parallel.ForEach(Users, kvp =>
            {
                var user = kvp.Value;
                _userUpdate?.Invoke(user);
                user.Network.SendSocketData();
            });
            _userUpdate = null;
        }

        private static void HandleTimers()
        {
            for (var i = 0; i < Timers.Count; i++)
            {
                var timer = Timers[i];
                if (timer.Item1 <= WorldTime.TickCount)
                {
                    timer.Item2();
                    Timers.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void OnUpdate(Action act)
        {
            using (TimedLock.Lock(_updateLock))
                _onUpdate += act;
        }

        public static void BroadcastAllUsers(Action<User> act)
        {
            OnUpdate(() => _userUpdate += act);
        }

        public static void AddTimedAction(long time, Action action)
        {
            Timers.Add(Tuple.Create(WorldTime.TickCount + TicksFromTime(time), action));
        }

        public static long TicksFromTime(long time)
        {
            return time / (1000 / TPS);
        }

        public static void ConnectUser(User user)
        {
            Users.Add(user);
        }

        private static void OnUserAdded(User user)
        {
            user.StartNetwork();
        }

        public static void DisconnectUser(User user)
        {
            Users.Remove(user);
            UserAccIds.Remove(user, out _);
        }

        public static bool TryDisconnectUserByName(string name)
        {
            var user = Users.FirstOrDefault(x => x.Value.Account.Name == name).Value;
            if (user == null)
                return false;

            user.Disconnect();
            return true;
        }

        public static async Task AddWorld(World world)
        {
            Worlds.TryAdd(world.Id, world);

            world.Active = true;
            world.Reset();
            await Task.Run(world.Initialize);
        }

        public static void RemoveWorld(World world)
        {
            Worlds.Remove(world.Id, out _);
            world.Dispose();
        }

        public static void OnRealmAdded(Realm realm)
        {
            ActiveRealms.TryAdd(realm.Id, realm.DisplayName);
            ChatManager.Announce($"A portal to {realm.DisplayName} has been opened.");
        }

        public static IEnumerable<Player> GetActivePlayers()
        {
            return Users.Where(user => user.Value.State == ConnectionState.Ready && user.Value.GameInfo.State == GameState.Playing)
                .Select(user => user.Value.GameInfo.Player);
        }

        public static World GetGuildHall(int guildId)
        {
            if (!_guildHalls.TryGetValue(guildId, out var ghall))
            {
                var guild = DbClient.GetGuild(guildId).Result;
                if (guild == null)
                    return null;

                ghall = _guildHalls[guildId] = new World("Guild Hall", guild.Level);
            }

            if (!ghall.Active || ghall.Deleted)
                AddWorld(ghall);

            return ghall;
        }

        public static void ReloadGuildHall(int guildId)
        {
            if (!_guildHalls.TryGetValue(guildId, out var ghall))
                return;

            ghall.Delete(); // This will disconnect all players

            var guild = DbClient.GetGuild(guildId).Result;
            var newGhall = new World("Guild Hall", guild.Level);
            _guildHalls[guildId] = newGhall;
            AddWorld(newGhall);
        }

        public static bool ReloadAllBehaviors()
        {
            var success = BehaviorLibrary.Reload(GameServerConfig.Config.BehaviorsDir);
            if (success)
                foreach (var world in Worlds)
                {
                    foreach (var ent in world.Value.Entities)
                    {
                        var c = ent.Value as Character;
                        c?.LoadBehavior();
                    }
                }

            return success;
        }
    }
}