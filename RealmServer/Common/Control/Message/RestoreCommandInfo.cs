#region

using System;

#endregion

namespace Common.Control.Message
{
    public class RestoreCommandInfo
    {
        public string Name { get; set; }
        public DateTime? DateTime { get; set; }
        public TimeSpan? TimeAgo { get; set; }
        public TimeSpan ShutdownDelay { get; set; } = TimeSpan.Zero;
    }
}