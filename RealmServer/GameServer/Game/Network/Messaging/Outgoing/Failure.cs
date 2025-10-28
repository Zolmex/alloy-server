#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.FAILURE)]
    public class Failure : IOutgoingPacket
    {
        public const string DEFAULT_MESSAGE = "An error occured while processing data from your client.";
        public const int DEFAULT = 0;
        public const int INCORRECT_VERSION = 1;
        public const int FORCE_CLOSE_GAME = 2;
        public const int INVALID_TELEPORT_TARGET = 3;
        public const int ACCOUNT_IN_USE = 4;
        public const int PORTAL_DISABLED = 5;

        public int ErrorId { get; }
        public string ErrorDescription { get; }

        public static void Write(NetworkHandler network, int errorId, string errorDescription)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(errorId);
                wtr.WriteUTF(errorDescription);

                state.PacketEnd(begin, PacketId.FAILURE);
            }
        }

        public override string ToString()
        {
            var type = typeof(Failure);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}