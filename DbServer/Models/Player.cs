// =============================================================================
//  LiteDB Player Document
// -----------------------------------------------------------------------------
//  This is the *persisted* representation of a player. It lives ONLY inside
//  DbServer - the GameServer never sees this type. Clients speak to us
//  exclusively via the gRPC domain commands declared in db_service.proto.
//
//  LiteDB serializes this class to a BSON document by convention (one document
//  per instance). [BsonId] marks the primary key; everything else is mapped
//  public-property-name <=> bson-field-name automatically.
//
//  We intentionally keep the model schema-less-friendly: new fields can be
//  added without writing migrations because LiteDB simply writes whatever is
//  present on the object. Old documents missing new fields will deserialize
//  with the default value (e.g. 0, null) for them.
// =============================================================================

using LiteDB;

namespace DbServer.Models;

/// <summary>
/// The Player document persisted in the <c>players</c> LiteDB collection.
/// </summary>
public class Player {
    /// <summary>
    /// Primary key / document id for the <c>players</c> collection. LiteDB
    /// uses this to look up documents by <c>Query.EQ("_id", value)</c>. We use
    /// the same int32 id that the proto commands reference (<c>player_id</c>).
    /// </summary>
    [BsonId]
    public int Id { get; set; }

    // ---- Stats (mutated by UpdatePlayerStats command) ---------------------- //
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int Level { get; set; }
    public long Experience { get; set; }

    // ---- Transform (mutated by SavePlayerTransform command) ----------------- //
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float RotationY { get; set; }

    // ---- Housekeeping ------------------------------------------------------- //
    /// <summary>UTC timestamp of the last successful command applied.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
