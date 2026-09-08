using System.Runtime.CompilerServices;
using Arch.Core;
using Common.Utilities.Collections;

namespace GameServer.Game.Worlds;

internal sealed class SpatialQueryCache
{
    private const float CACHE_CELL_SIZE = 2f; // quantization grid; tune to taste

    private readonly record struct CacheKey(int CellX, int CellY, float RadiusSqr);
    private readonly record struct CacheEntry(uint Generation, Entity[] Entities, int Count);

    private readonly Dictionary<CacheKey, CacheEntry> _cache = new(256);
    private uint _generation;

    public uint Generation => _generation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invalidate() => _generation++;

    public Entity[] GetOrCompute(float x, float y, float radiusSqr,
                               Func<Entity[]> compute, out int count)
    {
        var key = MakeKey(x, y, radiusSqr);
        if (_cache.TryGetValue(key, out var entry) && entry.Generation == _generation)
        {
            count = entry.Count;
            return entry.Entities;
        }

        var pooled = compute();
        count = 0;
        return pooled;
    }

    public Entity[] GetOrComputeWithCount(float x, float y, float radiusSqr,
                                        Func<(Entity[] entities, int count)> compute,
                                        out int count)
    {
        var key = MakeKey(x, y, radiusSqr);
        if (_cache.TryGetValue(key, out var entry) && entry.Generation == _generation)
        {
            count = entry.Count;
            return entry.Entities;
        }

        var (pooledIds, freshCount) = compute();
        count = freshCount;

        // Copy to a cache-owned array so we can return pooledIds to ArrayPool.
        Entity[] owned;
        if (freshCount == 0)
        {
            owned = [];
        }
        else
        {
            owned = new Entity[freshCount];
            pooledIds.AsSpan(0, freshCount).CopyTo(owned);
            System.Buffers.ArrayPool<Entity>.Shared.Return(pooledIds);
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