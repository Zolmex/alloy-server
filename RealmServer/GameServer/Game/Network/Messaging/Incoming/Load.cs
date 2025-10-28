#region

using Common.Database;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.LOAD)]
    public class Load : IIncomingPacket
    {
        public int CharId;

        public void Read(NetworkReader rdr)
        {
            CharId = rdr.ReadInt32();
        }

        public async void Handle(User user)
        {
            if (user.Account.Banned)
            {
                user.SendFailure(Failure.DEFAULT, "Account has been banned.");
                return;
            }

            var chr = user.GameInfo.Char;
            if (user.State != ConnectionState.Reconnecting)
            {
                chr = await DbClient.GetChar(user.Account.AccountId, CharId);
                if (chr == null)
                {
                    user.SendFailure(Failure.DEFAULT, $"Failed to load character #{CharId}");
                    return;
                }
            }

            if (chr == null)
            {
                user.SendFailure(Failure.DEFAULT, "Invalid reconnect state.");
                return;
            }

            if (chr.Dead)
            {
                user.SendFailure(Failure.DEFAULT, "Character is dead.");
                return;
            }

            var world = user.GameInfo.World;
            if (world == null || world.Deleted || !world.Active)
            {
                user.SendFailure(Failure.DEFAULT, "Invalid world.");
                return;
            }

            user.Load(chr, world);
        }

        public override string ToString()
        {
            var type = typeof(Load);
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