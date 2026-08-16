using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components.Data;

public record struct DamageRecord : IEntityIdentifiable {
    public EntityId Id { get; set; }
    
    public int DamageDealt;
    public readonly int FromAccId;
    
    public DamageRecord(EntityId fromId, int damageDealt, int fromAccId) {
        Id = fromId;
        DamageDealt = damageDealt;
        FromAccId = fromAccId;
    }
}