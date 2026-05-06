using Common.Database;
using Common.Network;
using Common.Utilities;
using GameServer.Game.Network;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.LOAD)]
public record Load : IIncomingPacket {
    public int CharId;

    public void Read(ref SpanReader rdr) {
        CharId = rdr.ReadInt32();
    }

    public async Task Handle(User user) {
        if (user.GameInfo.Account.IsBanned) {
            user.SendFailure(Failure.DEFAULT, "Account has been banned.");
            return;
        }

        var chr = user.GameInfo.Char;
        if (user.State != ConnectionState.Reconnecting) {
            chr = (await DbClient.GetCharacterAsync(user.GameInfo.Account.Id, CharId)).Character;
            if (chr == null) {
                user.SendFailure(Failure.DEFAULT, $"Failed to load character #{CharId}");
                return;
            }
        }

        if (chr == null) {
            user.SendFailure(Failure.DEFAULT, "Invalid reconnect state.");
            return;
        }

        if (chr.IsDead) {
            user.SendFailure(Failure.DEFAULT, "Character is dead.");
            return;
        }

        user.GameInfo.World?.Enqueue(() => {
            var world = user.GameInfo.World;
            if (world.Deleted) {
                user.SendFailure(Failure.DEFAULT, "Invalid world.");
                return;
            }

            user.Load(chr, world);
        });
    }
}