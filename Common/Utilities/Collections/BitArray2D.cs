using Common.Structs;
using System;
using System.Buffers;

public struct BitArray2D : IDisposable
{
    private readonly ulong[] _bits;
    private readonly int _width;
    private readonly int _stride;

    public BitArray2D(int width, int height)
    {
        _width = width;
        _stride = (width + 63) >> 6;
        _bits = ArrayPool<ulong>.Shared.Rent(_stride * height);
        _bits.AsSpan().Clear();
    }

    public bool Add(IntPoint p)
    {
        var idx = p.Y * _stride + (p.X >> 6);
        var mask = 1UL << (p.X & 63);
        if ((_bits[idx] & mask) != 0) return false;
        _bits[idx] |= mask;
        return true;
    }

    public bool Contains(IntPoint p)
    {
        var mask = 1UL << (p.X & 63);
        return (_bits[p.Y * _stride + (p.X >> 6)] & mask) != 0;
    }

    public void Dispose() => ArrayPool<ulong>.Shared.Return(_bits);
}