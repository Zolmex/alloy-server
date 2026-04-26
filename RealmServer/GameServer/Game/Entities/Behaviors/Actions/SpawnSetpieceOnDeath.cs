using GameServer.Game.Entities.Types;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record SpawnSetpieceOnDeath : BehaviorScript {
    private readonly string _setpiece;
    private readonly bool _useSpawnPoint;

    public SpawnSetpieceOnDeath(string setpiece, bool useSpawnPoint) {
        _setpiece = setpiece;
        _useSpawnPoint = useSpawnPoint;
    }

    public override void Start(CharacterEntity host) {
        host.DeathEvent += OnDeath;
    }

    private void OnDeath(Entity en) {
        var pos = _useSpawnPoint ? en.SpawnPosition : en.Position;
        en.World.SpawnSetPiece(_setpiece, (int)pos.X, (int)pos.Y, center: true);
    }
}