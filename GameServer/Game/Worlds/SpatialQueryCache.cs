using System.Runtime.CompilerServices;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

internal sealed class SpatialQueryCache
{
    private const float CACHE_CELL_SIZE = 2f; // quantization grid; tune to taste

    private readonly record struct CacheKey(int CellX, int CellY, float RadiusSqr);
    private readonly record struct CacheEntry(uint Generation, EntityId[] Ids, int Count);

    private readonly Dictionary<CacheKey, CacheEntry> _cache = new(256);
    private uint _generation;

    public uint Generation => _generation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invalidate() => _generation++;

    public EntityId[] GetOrCompute(float x, float y, float radiusSqr,
                               Func<EntityId[]> compute, out int count)
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

    public EntityId[] GetOrComputeWithCount(float x, float y, float radiusSqr,
                                        Func<(EntityId[] ids, int count)> compute,
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
        EntityId[] owned;
        if (freshCount == 0)
        {
            owned = [];
        }
        else
        {
            owned = new EntityId[freshCount];
            pooledIds.AsSpan(0, freshCount).CopyTo(owned);
            System.Buffers.ArrayPool<EntityId>.Shared.Return(pooledIds);
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