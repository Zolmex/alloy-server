namespace Common.Control
{
    public enum MemberType
    {
        GameServer,
        AppEngine,
        RealmBots,
        DatabaseManager
    }

    public class ServerMember
    {
        public MemberType Type { get; set; }
        public string Name { get; set; }
        public string InstanceID { get; set; }
        public ServerInfo ServerInfo { get; set; }
    }

    public class ServerInfo
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public int MaxPlayers { get; set; }
        public bool AdminOnly { get; set; }
        public PlayerInfo[] Players { get; set; }
    }

    public class PlayerInfo
    {
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string WorldName { get; set; }
    }
}