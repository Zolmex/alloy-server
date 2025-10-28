namespace Common.Utilities
{
    public interface ILogger
    {
        void Info(object message, bool saveToFile = false);
        void Warn(object message, bool saveToFile = false);
        void Error(object message, bool saveToFile = false);
        void Fatal(object message, bool saveToFile = false);
        void Debug(object message, bool saveToFile = false);
    }
}