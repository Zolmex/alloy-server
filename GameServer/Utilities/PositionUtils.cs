using System.Numerics;
using Common.Structs;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Components;
using GameServer.Game.Worlds;

namespace GameServer.Utilities;

public static class PositionUtils {
    extension(ref Position pos) {
        public float DistSqr(float x, float y) {
            return DistSqr(pos.Pos.X, pos.Pos.Y, x, y);
        }

        public float DistSqr(ref Position b) {
            var dx = pos.Pos.X - b.Pos.X;
            var dy = pos.Pos.Y - b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(int tileX, int tileY) {
            var dx = (int)pos.Pos.X - tileX;
            var dy = (int)pos.Pos.Y - tileY;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(ref Position b) {
            var dx = (int)pos.Pos.X - (int)b.Pos.X;
            var dy = (int)pos.Pos.Y - (int)b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public float GetAngleBetween(Vector2 vec) {
            return pos.GetAngleBetween(vec.X, vec.Y);
        }

        public float GetAngleBetween(ref Position b) {
            return pos.GetAngleBetween(b.Pos.X, b.Pos.Y);
        }

        public float GetAngleBetween(float x, float y) {
            return MathF.Atan2(y - pos.Pos.Y, x - pos.Pos.X);
        }

        public float GetDistanceBetween(ref Position entity2) //the diagonal distance
        {
            return GetDistanceBetween(pos.Pos.X, entity2.Pos.X, pos.Pos.Y, entity2.Pos.Y);
        }
    }

    public static float GetDistanceBetween(WorldPosData pos1, WorldPosData pos2) //the diagonal distance
        => GetDistanceBetween(pos1.X, pos2.X, pos1.Y, pos2.Y);
    
    public static float GetDistanceBetween(float x1, float x2, float y1, float y2) //the diagonal distance
    {
        var dx = MathF.Abs(x1 - x2);
        var dy = MathF.Abs(y1 - y2);
        return MathF.Sqrt(dx * dx + dy * dy);
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