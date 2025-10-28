namespace Common.Control
{
    public enum ControlChannel
    {
        MemberEnter,
        MemberLeave,
        KeepAlive,

        DbRestore,
        DbBackup,
        DbWipe,

        Shutdown
    }

    public class ControlMessage<T>
    {
        public string InstanceID { get; set; }
        public string TargetID { get; set; }
        public T Content { get; set; }
    }
}