#region

#endregion

using Common.Network;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERTEXT)]
public partial record PlayerText : IIncomingPacket
{
    public string Text;

    public void Handle(User user)
    {
        // if (user.Account.Muted) // TODO: fix
        // {
        //     user.GameInfo.Player.SendError("You are muted.");
        //     return;
        // }

        user.GameInfo.Player.Speak(Text);
    }

    public void Read(NetworkReader rdr)
    {
        Text = rdr.ReadUTF();
    }
}