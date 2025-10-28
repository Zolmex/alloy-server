#region

using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.PARTY_INVITE)]
    public class PartyInvite : IIncomingPacket
    {
        private int _objId;

        public void Read(NetworkReader rdr)
        {
            _objId = rdr.ReadInt32();
        }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var acc = user.Account;

            // Shouldn't happen but make sure player is in party before inviting
            if (acc.PartyId == -1)
                return;

            var target = user.GameInfo.Player.World.GetPlayerById(_objId);
            target?.InviteToParty(user.GameInfo.Player);
        }

        public override string ToString()
        {
            var type = typeof(PartyInvite);
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