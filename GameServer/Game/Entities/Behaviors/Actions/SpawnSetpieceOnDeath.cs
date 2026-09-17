
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Events;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record SpawnSetpieceOnDeath : BehaviorScript {
    private readonly string _setpiece;
    private readonly bool _useSpawnPoint;

    public SpawnSetpieceOnDeath(string setpiece, bool useSpawnPoint) {
        _setpiece = setpiece;
        _useSpawnPoint = useSpawnPoint;
    }

    public override void Start(ref EntityContext host) {
        host.World.EventSystem.Subscribe(host.Entity, OnDeath);
    }

    private void OnDeath(ref DeathEvent evt) {
        ref var enPos = ref evt.World.Ecs.Get<Position>(evt.Entity);
        var pos = _useSpawnPoint ? enPos.SpawnPos : enPos.Pos;
        evt.World.Map.SpawnSetPiece(_setpiece, (int)pos.X, (int)pos.Y, center: true);
    }
}