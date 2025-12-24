#region

using Common.Network;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.USEPORTAL)]
public partial record UsePortal : IIncomingPacket
{
    public int ObjectId;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        if (!user.GameInfo.Player.World.Entities.TryGetValue(ObjectId, out var en) || en is not Portal portal)
            return;

        portal.LoadWorld(user);

        if (portal.PortalWorld == null)
            user.SendFailure(Failure.PORTAL_DISABLED, "Invalid world.", false);
        else if (portal.PortalWorld.Deleted)
            user.SendFailure(Failure.PORTAL_DISABLED, "World is deleted.", false);
        // else if (!portal.PortalWorld.Initialized)
        //     user.SendFailure(Failure.PORTAL_DISABLED, "World has not initialized.", false);
        else if (!portal.Usable)
            user.SendFailure(Failure.PORTAL_DISABLED, "Portal disabled.", false);
        else
            user.ReconnectTo(portal.PortalWorld);
    }

    public void Read(NetworkReader rdr)
    {
        ObjectId = rdr.ReadInt32();
    }
}