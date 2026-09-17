using Common;
using Common.Structs;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Old;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record Flash : BehaviorScript {
    private readonly int _color;
    private readonly float _flashPeriod;
    private readonly int _flashRepeats;

    public Flash(int color, double flashPeriod, int flashRepeats) {
        _color = color;
        _flashPeriod = (float)flashPeriod;
        _flashRepeats = flashRepeats;
    }

    public override void Start(ref EntityContext host) {
        var en = host.Entity;
        host.World.Map.BroadcastNearby(host.Position.Pos, 20f, user => {
            user.SendPacket(new
                ShowEffect(
                    (byte)ShowEffectIndex.Flash,
                    (EntityId)en,
                    _color,
                    0,
                    new WorldPosData(_flashPeriod, _flashRepeats),
                    new WorldPosData()
                ));
        });
    }
}