using System.Numerics;
using Common.Structs;
using GameServer.Game.Entities;

namespace GameServer.Utilities;

public static class EntityUtils {
    extension(Entity en) {
        public float DistSqr(float x, float y) {
            return DistSqr(en.Pos.X, en.Pos.Y, x, y);
        }

        public float DistSqr(Entity b) {
            var dx = en.Pos.X - b.Pos.X;
            var dy = en.Pos.Y - b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(int tileX, int tileY) {
            var dx = (int)en.Pos.X - tileX;
            var dy = (int)en.Pos.Y - tileY;
            return dx * dx + dy * dy;
        }

        public double TileDistSqr(Entity b) {
            var dx = (int)en.Pos.X - (int)b.Pos.X;
            var dy = (int)en.Pos.Y - (int)b.Pos.Y;
            return dx * dx + dy * dy;
        }

        public float GetAngleBetween(Vector2 pos) {
            return en.GetAngleBetween(pos.X, pos.Y);
        }

        public float GetAngleBetween(Entity b) {
            return en.GetAngleBetween(b.Pos.X, b.Pos.Y);
        }

        public float GetAngleBetween(float x, float y) {
            return MathF.Atan2(y - en.Pos.Y, x - en.Pos.X);
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