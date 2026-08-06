// =============================================================================
//  DbServiceClient - GameServer-side wrapper around the gRPC DatabaseService.
// -----------------------------------------------------------------------------
//  This replaces the legacy `Common.Database.DbClient` (custom TCP model-diffing
//  protocol). It exposes a method per *domain command* in db_service.proto and
//  delegates to the generated `DatabaseServiceClient` that the proto compiler
//  emits from the `GrpcServices="Client"` selection.
//
//  The channel is created once at startup and reused for the lifetime of the
//  process (gRPC HTTP/2 multiplexes all calls over it). The DbServer address is
//  read from GameServerConfig or a sensible default; on Linux deployments this
//  usually points to `http://127.0.0.1:7001` (DbServer runs on the same host).
// =============================================================================

using DbServer.Protos;             // Generated proto types
using Grpc.Net.Client;              // Managed HTTP/2 client channel
using DatabaseServiceClient = DbServer.Protos.DatabaseService.DatabaseServiceClient;

namespace GameServer.Database;

public sealed class DbServiceClient : IDisposable {
    private readonly GrpcChannel           _channel;
    private readonly DatabaseServiceClient _client;

    public DbServiceClient(string address) {
        // GrpcChannel is thread-safe and connection-pooled. Creating a single
        // instance for the lifetime of the process is the recommended pattern.
        _channel = GrpcChannel.ForAddress(address);
        _client  = new DatabaseServiceClient(_channel);
    }

    // ----- UpdatePlayerStats -------------------------------------------------
    public async Task<CommandAck> UpdatePlayerStatsAsync(
        int playerId,
        int hpDelta   = 0,
        int mpDelta   = 0,
        int? level       = null,
        long? experience = null,
        CancellationToken ct = default) {

        var req = new UpdatePlayerStatsRequest {
            PlayerId  = playerId,
            HpDelta   = hpDelta,
            MpDelta   = mpDelta,
        };

        if (level.HasValue)      { req.HasLevel     = true; req.Level     = level.Value; }
        if (experience.HasValue) { req.HasExperience = true; req.Experience = experience.Value; }

        return await _client.UpdatePlayerStatsAsync(req, cancellationToken: ct);
    }

    // ----- SavePlayerTransform ----------------------------------------------
    public async Task<CommandAck> SavePlayerTransformAsync(
        int playerId,
        float x, float y, float z,
        float rotationY,
        CancellationToken ct = default) {

        var req = new SavePlayerTransformRequest {
            PlayerId   = playerId,
            Position   = new Vector3 { X = x, Y = y, Z = z },
            RotationY  = rotationY,
        };

        return await _client.SavePlayerTransformAsync(req, cancellationToken: ct);
    }

    public void Dispose() => _channel.Dispose();
}
