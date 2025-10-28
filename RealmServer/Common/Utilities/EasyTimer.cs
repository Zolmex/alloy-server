#region

using System;
using System.Diagnostics;

#endregion

namespace Common.Utilities
{
    public class EasyTimer : IDisposable
    {
        public const string Time = "[TIME]";
        private static readonly Logger Log = new(typeof(EasyTimer));

        private readonly LogLevel _level;
        private readonly Stopwatch _sw;
        private readonly string _finalMessage;

        public EasyTimer(LogLevel level = LogLevel.Debug, string firstMessage = null, string finalMessage = "[TIME]")
        {
            if (firstMessage != null)
                Log.Log(level, firstMessage);
            _level = level;
            _finalMessage = finalMessage;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            Log.Log(_level, _finalMessage.Replace(Time, _sw.Elapsed.TotalMilliseconds + " ms"));
        }
    }
}