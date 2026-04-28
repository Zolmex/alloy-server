using System.Diagnostics;
using Common.Utilities;

namespace GameServer.Game;

public struct RealmTime {
    public float TickCountDecimal;
    public long TickCount;
    public long TotalElapsedMs;
    public int ElapsedMsDelta;
}

public class RealmManager {
    private static readonly Logger _log = new(typeof(RealmManager));

    public static RealmTime WorldTime;
    
    public static void Init() {
        _log.Info("Realm Manager initialized.");
    }

    public static void Run(int mspt) {
        var lagMs = (int)(mspt * 1.5);
        var sw = Stopwatch.StartNew();
        while (true) {
            if (sw.ElapsedMilliseconds < mspt)
                continue;

            WorldTime.ElapsedMsDelta = (int)sw.ElapsedMilliseconds;
            WorldTime.TotalElapsedMs += sw.ElapsedMilliseconds;
            WorldTime.TickCountDecimal += WorldTime.ElapsedMsDelta / (float)mspt;
            if (WorldTime.TickCountDecimal > 1) {
                var ticks = (int)WorldTime.TickCountDecimal;
                WorldTime.TickCountDecimal -= ticks;
                WorldTime.TickCount += ticks;
            }

            if (WorldTime.ElapsedMsDelta >= lagMs)
                _log.Warn($"LAGGED | MsPT: {mspt} Elapsed: {WorldTime.ElapsedMsDelta}");

            sw.Restart();
        }
    }
}