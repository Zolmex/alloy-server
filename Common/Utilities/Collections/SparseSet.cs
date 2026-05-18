using System;
using System.Buffers;

namespace Common.Utilities.Collections;

public sealed class SparseSet<T> : IDisposable where T : struct, IEntityIdentifiable {
    public int Count;

    private int[] _sparse;
    private int[] _generations; // parallel to _sparse, stores expected generation
    private T[] _dense;
    private bool _proj;

    public SparseSet(int sparseCapacity = 10, int denseCapacity = 10, bool projectiles = false) {
        _proj = projectiles;
        _sparse = ArrayPool<int>.Shared.Rent(sparseCapacity);
        _generations = ArrayPool<int>.Shared.Rent(sparseCapacity);
        Array.Fill(_sparse, 0);
        Array.Fill(_generations, 0);
        _dense = ArrayPool<T>.Shared.Rent(denseCapacity);
        _dense[0] = default;
        Count = 1;
    }

    public ref T Add(ref T elem) {
        var index = elem.Id.Index;

        if (index >= _sparse.Length)
            ResizeSparse(index + 1);

        if (Count >= _dense.Length)
            ResizeDense(Count * 2);

        _sparse[index] = Count;
        _generations[index] = elem.Id.Generation;
        _dense[Count] = elem;
        return ref _dense[Count++];
    }

    public ref T GetOrAdd(EntityId id, out bool added) {
        if (id.Index >= _sparse.Length)
            ResizeSparse(id.Index + 1);

        var idx = _sparse[id.Index];
        if (idx != 0 && _generations[id.Index] == id.Generation) {
            added = false;
            return ref _dense[idx];
        }

        if (Count >= _dense.Length)
            ResizeDense(Count * 2);

        _sparse[id.Index] = Count;
        _generations[id.Index] = id.Generation;
        _dense[Count] = default;
        added = true;
        return ref _dense[Count++];
    }

    public bool Remove(EntityId id, out T elem) {
        elem = default;
        if (id.Index < 0 || id.Index >= _sparse.Length)
            return false;

        var indexInDense = _sparse[id.Index];
        if (indexInDense == 0 || _generations[id.Index] != id.Generation)
            return false;

        elem = _dense[indexInDense];
        _sparse[id.Index] = 0;
        _generations[id.Index] = 0;
        Count--;

        if (indexInDense == Count)
            return true;

        ref var slot = ref _dense[indexInDense];
        slot = _dense[Count];
        _sparse[slot.Id.Index] = indexInDense;
        _generations[slot.Id.Index] = slot.Id.Generation;
        _dense[Count] = default;
        return true;
    }

    public ref T Get(EntityId id) {
        if (id.Index < 0 || id.Index >= _sparse.Length)
            return ref _dense[0];

        var idx = _sparse[id.Index];
        if (_generations[id.Index] != id.Generation)
            return ref _dense[0]; // Stale reference, return invalid slot
        
        return ref _dense[idx];
    }

    public ref T GetAt(int denseIndex) {
        if (denseIndex < 0 || denseIndex >= _dense.Length)
            return ref _dense[0];
        return ref _dense[denseIndex];
    }

    public int MoveNextGeneration(int index) {
        if (index < 0)
            return 0;
        
        if (index >= _sparse.Length)
            ResizeSparse(index + 1);
        
        return ++_generations[index];
    }

    private void ResizeSparse(int capacity) {
        var newSparse = ArrayPool<int>.Shared.Rent(capacity);
        var newGenerations = ArrayPool<int>.Shared.Rent(capacity);
        _sparse.AsSpan().CopyTo(newSparse);
        _generations.AsSpan().CopyTo(newGenerations);
        newSparse.AsSpan(_sparse.Length).Fill(0);
        newGenerations.AsSpan(_generations.Length).Fill(0);
        ArrayPool<int>.Shared.Return(_sparse);
        ArrayPool<int>.Shared.Return(_generations);
        _sparse = newSparse;
        _generations = newGenerations;
    }

    private void ResizeDense(int capacity) {
        if (capacity < _dense.Length)
            throw new ArgumentException($"New capacity cannot be lower than the current: {capacity} < {_dense.Length}");

        var newDense = ArrayPool<T>.Shared.Rent(capacity);
        _dense.AsSpan().CopyTo(newDense);
        ArrayPool<T>.Shared.Return(_dense);
        _dense = newDense;
    }

    public void Dispose() {
        ArrayPool<int>.Shared.Return(_sparse);
        ArrayPool<int>.Shared.Return(_generations);
        ArrayPool<T>.Shared.Return(_dense);
    }

    public SparseEnumerator<T> GetEnumerator() => new(this);
}