using Common.Utilities;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Components.Data;

public record struct DamageRecord : IEntityIdentifiable {
    public EntityId Id { get; set; }
    
    public int DamageDealt;
    
    public DamageRecord(EntityId fromId, int damageDealt) {
        Id = fromId;
        DamageDealt = damageDealt;
    }
}