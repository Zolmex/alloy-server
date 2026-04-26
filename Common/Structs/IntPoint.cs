namespace Common.Structs;

public struct IntPoint {
    public int X;
    public int Y;

    public IntPoint(int x, int y) {
        X = x;
        Y = y;
    }

    public static bool operator ==(IntPoint a, IntPoint b) {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(IntPoint a, IntPoint b) {
        return a.X != b.X || a.Y != b.Y;
    }

    public bool Equals(IntPoint other) {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj) {
        if (obj is IntPoint p) return Equals(p);

        return false;
    }

    public override int GetHashCode() {
        return (Y << 16) ^ X;
    }

    public override string ToString() {
        return $"X:{X}, Y:{Y}";
    }
}