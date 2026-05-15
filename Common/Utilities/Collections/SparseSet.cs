using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities.Collections;

public sealed class SparseSet<T> : IDisposable where T : struct, IIdentifiable{

    public int Count;

    private int[] _sparse;
    private T[] _dense;

    public SparseSet(int sparseCapacity = 10, int denseCapacity = 10) {
        _sparse = ArrayPool<int>.Shared.Rent(sparseCapacity);
        Array.Fill(_sparse, 0);
        _dense = ArrayPool<T>.Shared.Rent(denseCapacity);
        _dense[0] = default; // Invalid accesses return this, user must treat it as null
        Count = 1;
    }

    public ref T Add(ref T elem) {
        if (elem.Id >= _sparse.Length) {
            ResizeSparse(elem.Id + 1);
        }
    
        if (Count >= _dense.Length) {
            ResizeDense(Count * 2);
        }

        _sparse[elem.Id] = Count;
        _dense[Count] = elem; // Copies element
        return ref _dense[Count++];
    }
    
    public ref T GetOrAdd(int id, out bool added) {
        if (id >= _sparse.Length)
            ResizeSparse(id + 1);

        var idx = _sparse[id];
        if (idx != 0) {
            added = false;
            return ref _dense[idx];
        }

        // Insert new
        if (Count >= _dense.Length)
            ResizeDense(Count * 2);

        _sparse[id] = Count;
        _dense[Count] = default;
        added = true;
        return ref _dense[Count++];
    }

    public bool Remove(int id, out T elem) {
        elem = default;
        if (id < 0 || id >= _sparse.Length)
            return false;
        
        var indexInDense = _sparse[id];
        if (indexInDense == 0)
            return false;
        
        elem = _dense[indexInDense];
        _sparse[id] = 0; // Point to default
        Count--;
        
        if (indexInDense == Count) // Was the last element, no swap needed
            return true;
        
        ref var slot = ref _dense[indexInDense];
        slot = _dense[Count]; // Swap with last element
        _sparse[slot.Id] = indexInDense;
        _dense[Count] = default;
        return true;
    }

    public ref T Get(int id) {
        if (id < 0 || id >= _sparse.Length)
            return ref _dense[0];
        return ref _dense[_sparse[id]];
    }
    
    public ref T GetAt(int denseIndex) {
        if (denseIndex < 0 || denseIndex >= _dense.Length)
            return ref _dense[0];
        return ref _dense[denseIndex];
    }

    private void ResizeSparse(int capacity) {
        var newSparse = ArrayPool<int>.Shared.Rent(capacity);
        _sparse.AsSpan().CopyTo(newSparse);
        newSparse.AsSpan(_sparse.Length).Fill(0); // Clear the new region
        ArrayPool<int>.Shared.Return(_sparse);
        _sparse = newSparse;
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
        ArrayPool<T>.Shared.Return(_dense);
    }

    public SparseEnumerator<T> GetEnumerator() => new(this);
}