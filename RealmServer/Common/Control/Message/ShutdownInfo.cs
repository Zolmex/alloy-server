#region

using System;

#endregion

namespace Common.Control.Message
{
    public class ShutdownInfo
    {
        public TimeSpan ShutdownDelay { get; set; } = TimeSpan.Zero;
        public string Reason { get; set; } = string.Empty;
    }
}