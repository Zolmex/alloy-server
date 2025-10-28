#region

using Common.Database;
using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CREATE)]
    public class Create : IIncomingPacket
    {
        public short ClassType;
        public short SkinType;

        public void Read(NetworkReader rdr)
        {
            ClassType = rdr.ReadInt16();
            SkinType = rdr.ReadInt16();
        }

        public async void Handle(User user)
        {
            var createChar = await DbClient.CreateCharacter(user.Account, ClassType, SkinType);
            var chr = createChar.Item1;
            var result = createChar.Item2;
            if (chr == null)
            {
                user.SendFailure(Failure.DEFAULT, result.GetDescription());
                return;
            }
            else
            {
                var world = user.GameInfo.World;
                if (world == null || world.Deleted)
                {
                    user.SendFailure(Failure.DEFAULT, "World does not exist.");
                    return;
                }

                user.Load(chr, world);
            }
        }

        public override string ToString()
        {
            var type = typeof(Create);
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