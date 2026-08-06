// =============================================================================
//  DbCommandExample
// -----------------------------------------------------------------------------
//  Demonstrates: (1) initializing the gRPC client channel, and
//                (2) firing a domain command off to the DbServer.
//
//  This is intentionally a small, runnable example you can lift pieces from.
//  It mirrors the convenience-call style of the old `DbClient` static class
//  so the rest of GameServer can keep using the same patterns.
// =============================================================================

using Common.Utilities;             // The repo's existing Logger
using GameServer.Database;          // Our gRPC client wrapper

namespace GameServer.Game;

/// <summary>
/// Holds the shared `DbServiceClient` instance for the entire GameServer
/// process plus demo call sites. In real usage you would usually distribute
/// the client around via DI, but for this codebase's static-style bootstrap we
/// keep it accessible here as a singleton property.
/// </summary>
public static class DbCommandExample {
    private static readonly Logger _log = new(typeof(DbCommandExample));

    // Set once at startup (see Initialize). Null only before initialization.
    public static DbServiceClient Db { get; private set; }

    /// <summary>Wires up the gRPC channel to DbServer. Call once during boot.</summary>
    public static async Task InitializeAsync(string dbServerAddress) {
        Db = new DbServiceClient(dbServerAddress);
        _log.Info($"gRPC channel open to DbServer @ {dbServerAddress}");

        // ----- example 1: UpdatePlayerStats ------------------------------------
        // Player 42 gained 25 HP and reached level 5 (experience absolute).
        var statsAck = await Db.UpdatePlayerStatsAsync(playerId: 42, hpDelta: +25, level: 5, experience: 1234L);
        _log.Info($"UpdatePlayerStats -> success={statsAck.Success}, msg='{statsAck.Message}'");

        // ----- example 2: SavePlayerTransform ----------------------------------
        // Player 42 moved to (123.4, 56.0, -7.8) facing yaw=1.57 rad.
        var xformAck = await Db.SavePlayerTransformAsync(playerId: 42, x: 123.4f, y: 56.0f, z: -7.8f, rotationY: 1.57f);
        _log.Info($"SavePlayerTransform -> success={xformAck.Success}, msg='{xformAck.Message}'");
    }
}
