using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities.Collections;

public class SparseSet<T> where T : struct, IIdentifiable {

    public int Count;

    private int[] _sparse;
    private T[] _dense;

    public SparseSet(int capacity = 0) {
        _sparse = ArrayPool<int>.Shared.Rent(capacity);
        Array.Fill(_sparse, 0);
        _dense = ArrayPool<T>.Shared.Rent(capacity);
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

    public void Remove(int id) {
        var indexInDense = _sparse[id];
        if (indexInDense == 0)
            return;
        
        var lastElement = _dense[Count - 1];
        
        _dense[indexInDense] = lastElement;
        _sparse[lastElement.Id] = indexInDense;

        _sparse[id] = 0; // Point to default
        Count--;
    }

    public ref T Get(int id) {
        return ref _dense[_sparse[id]];
    }
    
    public ref T GetAt(int denseIndex) {
        return ref _dense[denseIndex];
    }

    private void ResizeSparse(int capacity) {
        if (capacity < _sparse.Length)
            throw new ArgumentException($"New capacity cannot be lower than the current: {capacity} < {_sparse.Length}");

        var newSparse = ArrayPool<int>.Shared.Rent(capacity);
        _sparse.AsSpan().CopyTo(newSparse);
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
}