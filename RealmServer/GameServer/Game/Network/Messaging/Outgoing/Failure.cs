#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct Failure(int ErrorId, string ErrorDescription) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.FAILURE;

    public const string DEFAULT_MESSAGE = "An error occured while processing data from your client.";
    public const int DEFAULT = 0;
    public const int INCORRECT_VERSION = 1;
    public const int FORCE_CLOSE_GAME = 2;
    public const int INVALID_TELEPORT_TARGET = 3;
    public const int ACCOUNT_IN_USE = 4;
    public const int PORTAL_DISABLED = 5;
}