using System;
using System.Numerics;
using Common.Network;

namespace Common.Structs;

public struct WorldPosData : IEquatable<WorldPosData> {
    public float X;
    public float Y;

    public WorldPosData(float x, float y) {
        X = x;
        Y = y;
    }

    public static WorldPosData Read(ref SpanReader rdr) {
        return new WorldPosData { X = rdr.ReadSingle(), Y = rdr.ReadSingle() };
    }


    public override int GetHashCode() {
        return (X, Y).GetHashCode();
    }

    public override bool Equals(object other) {
        return other is WorldPosData pos && Equals(pos);
    }

    public bool Equals(WorldPosData pos) {
        return pos.X == X &&
               pos.Y == Y;
    }

    public static bool operator ==(WorldPosData pos1, WorldPosData pos2) {
        return pos1.Equals(pos2);
    }

    public static bool operator !=(WorldPosData pos1, WorldPosData pos2) {
        return !pos1.Equals(pos2);
    }

    public static implicit operator Vector2(WorldPosData pos) {
        return new Vector2(pos.X, pos.Y);
    }
}

public static class WorldPosDataExtensions {
    public static Vector2 ToVec2(this WorldPosData data) {
        return new Vector2(data.X, data.Y);
    }

    public static float DistSqr(this Vector2 vec1, Vector2 vec2) {
        var dx = vec1.X - vec2.X;
        var dy = vec1.Y - vec2.Y;
        return dx * dx + dy * dy;
    }

    public static float AngleDegrees(this WorldPosData pos1, WorldPosData pos2) {
        return pos1.AngleRadians(pos2) * 180f / (float)Math.PI;
    }

    public static float AngleRadians(this WorldPosData pos1, WorldPosData pos2) {
        return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
    }
}