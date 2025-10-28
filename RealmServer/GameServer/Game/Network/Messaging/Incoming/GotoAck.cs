#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.GOTOACK)]
    public class GotoAck : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var plr = user.GameInfo.Player;
            if (!plr.Teleporting)
                return;

            plr.FinishTeleport();
        }

        public override string ToString()
        {
            var type = typeof(GuildRemove);
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