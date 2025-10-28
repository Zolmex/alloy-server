#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.ESCAPE)]
    public class Escape : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            if (user.GameInfo.World.Id == World.NEXUS_ID)
            {
                user.GameInfo.Player.SendInfo("You're already in the Nexus!");
                return;
            }

            user.ReconnectTo(RealmManager.NexusInstance);
        }

        public override string ToString()
        {
            var type = typeof(Escape);
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