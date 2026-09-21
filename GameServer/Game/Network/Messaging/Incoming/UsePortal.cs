using Arch.Core;
using Common.Network;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.USEPORTAL)]
public record UsePortal : IIncomingPacket {
    public EntityId ObjectId;

    public async Task Handle(User user) {
        if (user.Session.State != SessionState.Playing)
            return;

        var portal = user.Session.World.GetEntity(ObjectId);
        if (portal == Entity.Null)
            return;
        
        ref var portalData = ref user.Session.World.Ecs.Get<PortalData>(portal);
        var world = portalData.GetInstance(user);
        if (world == null)
            user.SendFailure(Failure.PORTAL_DISABLED, "Invalid world.", false);
        else if (world.Deleted)
            user.SendFailure(Failure.PORTAL_DISABLED, "World is deleted.", false);
        else if (portalData.Locked)
            user.SendFailure(Failure.PORTAL_DISABLED, "Portal is locked.", false);
        else
            user.ReconnectTo(world);
    }

    public void Read(ref SpanReader rdr) {
        ObjectId = EntityId.Read(ref rdr);
    }
}