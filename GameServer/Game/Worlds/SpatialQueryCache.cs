using System.Runtime.CompilerServices;

namespace GameServer.Game.Worlds;

internal sealed class SpatialQueryCache
{
    private const float CACHE_CELL_SIZE = 2f; // quantization grid; tune to taste

    private readonly record struct CacheKey(int CellX, int CellY, float RadiusSqr);
    private readonly record struct CacheEntry(uint Generation, int[] Ids, int Count);

    private readonly Dictionary<CacheKey, CacheEntry> _cache = new(256);
    private uint _generation;

    public uint Generation => _generation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invalidate() => _generation++;

    public int[] GetOrCompute(float x, float y, float radiusSqr,
                               Func<int[]> compute, out int count)
    {
        var key = MakeKey(x, y, radiusSqr);
        if (_cache.TryGetValue(key, out var entry) && entry.Generation == _generation)
        {
            count = entry.Count;
            return entry.Ids;
        }

        var pooled = compute();
        count = 0;
        return pooled;
    }

    public int[] GetOrComputeWithCount(float x, float y, float radiusSqr,
                                        Func<(int[] ids, int count)> compute,
                                        out int count)
    {
        var key = MakeKey(x, y, radiusSqr);
        if (_cache.TryGetValue(key, out var entry) && entry.Generation == _generation)
        {
            count = entry.Count;
            return entry.Ids;
        }

        var (pooledIds, freshCount) = compute();
        count = freshCount;

        // Copy to a cache-owned array so we can return pooledIds to ArrayPool.
        int[] owned;
        if (freshCount == 0)
        {
            owned = [];
        }
        else
        {
            owned = new int[freshCount];
            pooledIds.AsSpan(0, freshCount).CopyTo(owned);
            System.Buffers.ArrayPool<int>.Shared.Return(pooledIds);
        }

        _cache[key] = new CacheEntry(_generation, owned, freshCount);
        return owned;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CacheKey MakeKey(float x, float y, float radiusSqr)
    {
        var cx = (int)(x / CACHE_CELL_SIZE);
        var cy = (int)(y / CACHE_CELL_SIZE);
        return new CacheKey(cx, cy, radiusSqr);
    }
}