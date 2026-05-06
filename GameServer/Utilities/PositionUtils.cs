using System.Numerics;
using Common.Structs;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;

namespace GameServer.Utilities;

public static class PositionUtils {
    extension(ref StatsComponent stats) {
        public float DistSqr(float x, float y) {
            return DistSqr(stats.Pos.X, stats.Pos.Y, x, y);
        }

        public float DistSqr(ref StatsComponent b) {
            var dx = stats.Pos.X - b.Pos.X;
            var dy = stats.Pos.Y - b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(int tileX, int tileY) {
            var dx = (int)stats.Pos.X - tileX;
            var dy = (int)stats.Pos.Y - tileY;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(ref StatsComponent b) {
            var dx = (int)stats.Pos.X - (int)b.Pos.X;
            var dy = (int)stats.Pos.Y - (int)b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public float GetAngleBetween(Vector2 pos) {
            return stats.GetAngleBetween(pos.X, pos.Y);
        }

        public float GetAngleBetween(ref StatsComponent b) {
            return stats.GetAngleBetween(b.Pos.X, b.Pos.Y);
        }

        public float GetAngleBetween(float x, float y) {
            return MathF.Atan2(y - stats.Pos.Y, x - stats.Pos.X);
        }
    }
    
    public static float DistSqr(WorldPosData pos1, WorldPosData pos2) {
        return DistSqr(pos1.X, pos1.Y, pos2.X, pos2.Y);
    }

    public static float DistSqr(Vector2 a, Vector2 b) {
        return DistSqr(a.X, a.Y, b.X, b.Y);
    }

    public static float DistSqr(float x1, float y1, float x2, float y2) {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return dx * dx + dy * dy;
    }
    
    public static float GetAngleBetween(this Vector2 pos, Vector2 pos2) {
        return MathF.Atan2(pos2.Y - pos.Y, pos2.X - pos.X);
    }
}