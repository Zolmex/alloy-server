#region

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct MapInfo(int MapWidth,
    int MapHeight,
    string Name,
    string DisplayName,
    uint Seed,
    int Background,
    bool ShowDisplays,
    bool AllowPlayerTeleport,
    string Music,
    int Difficulty) : IOutgoingPacket
{
    static PacketId IOutgoingPacket.PacketId => PacketId.MAPINFO;
}