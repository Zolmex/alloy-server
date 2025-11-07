#region

using Common;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing;

public readonly partial record struct ShowEffect(byte EffectType, int TargetId, int Color, float EffectParam, WorldPosData Pos1, WorldPosData Pos2) : IOutgoingPacket
{
}