#region

using Common.Database;
using Common.Network;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.CREATE)]
public partial record Create : IIncomingPacket
{
    public short ClassType;
    public short SkinType;

    public async void Handle(User user)
    {
        var createChar = await DbClient.CreateCharacter(user.Account, ClassType, SkinType);
        var chr = createChar.Item1;
        var result = createChar.Item2;
        if (chr == null)
        {
            user.SendFailure(Failure.DEFAULT, result.GetDescription());
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

    public void Read(NetworkReader rdr)
    {
        ClassType = rdr.ReadInt16();
        SkinType = rdr.ReadInt16();
    }
}