using DbServer.Models;
using DbServer.Protos;
using Grpc.Core;
using LiteDB;

namespace DbServer.Services;

public class DatabaseServiceImpl(LiteDatabase db) : DatabaseService.DatabaseServiceBase {
    private readonly ILiteCollection<Player> _players = db.GetCollection<Player>("players");

    public override Task<CommandAck> UpdatePlayerStats(UpdatePlayerStatsRequest req, ServerCallContext ctx) {
        var player = _players.FindById(req.PlayerId);
        if (player is null)
            player = new Player { Id = req.PlayerId };

        player.Hp += req.HpDelta;
        player.Mp += req.MpDelta;

        // Absolutes only when the matching flag is set; otherwise leave the
        // persisted value untouched.
        if (req.HasLevel)     player.Level     = req.Level;
        if (req.HasExperience) player.Experience = req.Experience;

        player.UpdatedAt = DateTime.UtcNow;

        _players.Upsert(player);

        return Task.FromResult(new CommandAck { Success = true, Message = "stats updated" });
    }

    public override Task<CommandAck> SavePlayerTransform(SavePlayerTransformRequest req, ServerCallContext ctx) {
        if (req.Position is null)
            return Task.FromResult(new CommandAck { Success = false, Message = "position required" });

        var player = _players.FindById(req.PlayerId);
        if (player is null)
            player = new Player { Id = req.PlayerId };

        player.X          = req.Position.X;
        player.Y          = req.Position.Y;
        player.Z          = req.Position.Z;
        player.RotationY  = req.RotationY;
        player.UpdatedAt  = DateTime.UtcNow;

        _players.Upsert(player);

        return Task.FromResult(new CommandAck { Success = true, Message = "transform saved" });
    }
}
