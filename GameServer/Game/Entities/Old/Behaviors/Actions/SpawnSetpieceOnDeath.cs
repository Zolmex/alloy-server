using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record SpawnSetpieceOnDeath : BehaviorScript {
    private readonly string _setpiece;
    private readonly bool _useSpawnPoint;

    public SpawnSetpieceOnDeath(string setpiece, bool useSpawnPoint) {
        _setpiece = setpiece;
        _useSpawnPoint = useSpawnPoint;
    }

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(OnDeath);
    }

    private void OnDeath(ref DeathEvent evt) {
        ref var enStats = ref evt.World.EntityStats.Get(evt.HostId);
        var pos = _useSpawnPoint ? enStats.SpawnPos : enStats.Pos;
        evt.World.Map.SpawnSetPiece(_setpiece, (int)pos.X, (int)pos.Y, center: true);
    }
}