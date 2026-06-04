using Common.Network;
using Common.Utilities.Collections;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.USEPORTAL)]
public record UsePortal : IIncomingPacket {
    public EntityId ObjectId;

    public async Task Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        ref var portalData = ref user.GameInfo.World.PortalDatas.Get(ObjectId);
        if (portalData.Id == EntityId.Null)
            return;

        var world = portalData.GetWorldInstance(user);
        if (world == null)
            user.SendFailure(Failure.PORTAL_DISABLED, "Invalid world.", false);
        else if (world.Deleted)
            user.SendFailure(Failure.PORTAL_DISABLED, "World is deleted.", false);
        else if (portalData.Disabled)
            user.SendFailure(Failure.PORTAL_DISABLED, "Portal disabled.", false);
        else
            user.ReconnectTo(world);
    }

    public void Read(ref SpanReader rdr) {
        ObjectId = EntityId.Read(ref rdr);
    }
}