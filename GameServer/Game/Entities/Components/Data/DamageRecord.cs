using Common.Utilities;

namespace GameServer.Game.Entities.Components.Data;

public record struct DamageRecord : IIdentifiable {
    public int Id { get; set; }
    
    public int FromId;
    public int DamageDealt;
    
    public DamageRecord(int fromId, int damageDealt) {
        Id = fromId;
        FromId = fromId;
        DamageDealt = damageDealt;
    }
}