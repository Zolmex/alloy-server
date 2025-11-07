#region

using Common;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Flash : BehaviorScript
{
    private readonly int _color;
    private readonly float _flashPeriod;
    private readonly int _flashRepeats;

    public Flash(int color, double flashPeriod, int flashRepeats)
    {
        _color = color;
        _flashPeriod = (float)flashPeriod;
        _flashRepeats = flashRepeats;
    }

    public override void Start(Character host)
    {
        host.World.BroadcastAll(p =>
        {
            p.User.SendPacket(new
            ShowEffect(
                (byte)ShowEffectIndex.Flash,
                host.Id,
                _color,
                0,
                new WorldPosData(_flashPeriod, _flashRepeats),
                new WorldPosData()
            ));
        });
    }
}