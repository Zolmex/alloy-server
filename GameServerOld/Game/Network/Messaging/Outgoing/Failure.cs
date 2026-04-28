using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Outgoing;

public readonly record struct Failure(int ErrorId, string ErrorDescription) : IOutgoingPacket {
    public const string DEFAULT_MESSAGE = "An error occured while processing data from your client.";
    public const int DEFAULT = 0;
    public const int INCORRECT_VERSION = 1;
    public const int FORCE_CLOSE_GAME = 2;
    public const int INVALID_TELEPORT_TARGET = 3;
    public const int ACCOUNT_IN_USE = 4;
    public const int PORTAL_DISABLED = 5;
    public PacketId ID => PacketId.FAILURE;

    public void Write(ref SpanWriter wtr) {
        wtr.Write(ErrorId);
        wtr.WriteUTF(ErrorDescription);
    }
}