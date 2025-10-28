#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CONSTELLATIONSSAVE)]
    public class ConstellationsSave : IIncomingPacket
    {
        public int SavedPrimaries;
        public int SavedSecondaries;

        public void Read(NetworkReader rdr)
        {
            SavedPrimaries = rdr.ReadInt32();
            SavedSecondaries = rdr.ReadInt32();
        }

        public void Handle(User user)
        {
            var player = user.GameInfo.Player;
            if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
                return;
            var primaries = player.ConvertNodeData(SavedPrimaries);
            var secondaries = player.ConvertNodeData(SavedSecondaries);
            for (var i = 0; i < 4; i++)
                if ((i == 0 && (primaries[i] > 4 || secondaries[i] > 4)) || (i != 0 && (primaries[i] > 3 || secondaries[i] > 3)))
                    return; //if large node is > 4 or small nodes are > 3

            player.PrimaryNodeData = SavedPrimaries;
            player.PrimaryNodes = primaries;
            player.SecondaryNodeData = SavedSecondaries;
            player.SecondaryNodes = secondaries;

            player.ReloadConstellationMods();
        }

        public override string ToString()
        {
            var type = typeof(ConstellationsSave);
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