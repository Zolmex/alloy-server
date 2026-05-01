using System;

namespace Common.Utilities.Collections;

public class SparseSet<T> where T : struct, IIdentifiable {

    public int Count;
    public int Capacity;

    private int[] _sparse;
    private T[] _dense;

    public SparseSet(int capacity = 0) {
        Capacity = capacity;
        _sparse = new int[capacity];
        Array.Fill(_sparse, -1);
        _dense = new T[capacity];
    }

    public void Add(ref T elem) {
        if (elem.Id >= _sparse.Length) {
            ResizeSparse(elem.Id + 1);
        }
    
        if (Count >= _dense.Length) {
            ResizeDense(Count * 2);
        }

        _sparse[elem.Id] = Count;
        _dense[Count] = elem;
        Count++;
    }

    public void Remove(int id) {
        var indexInDense = _sparse[id];
        var lastElement = _dense[Count - 1];
        
        _dense[indexInDense] = lastElement;
        _sparse[lastElement.Id] = indexInDense;

        _sparse[id] = -1;
        Count--;
    }

    public ref T Get(int id) {
        return ref _dense[_sparse[id]];
    }

    private void ResizeSparse(int capacity) {
        if (capacity < Capacity)
            throw new ArgumentException($"New capacity cannot be lower than the current: {capacity} < {Capacity}");

        Capacity = capacity;
        
        var newSparse = new int[capacity];
        _sparse.AsSpan().CopyTo(newSparse);
        _sparse = newSparse;
    }
    
    private void ResizeDense(int capacity) {
        if (capacity < Capacity)
            throw new ArgumentException($"New capacity cannot be lower than the current: {capacity} < {Capacity}");

        Capacity = capacity;
        
        var newDense = new T[capacity];
        _dense.AsSpan().CopyTo(newDense);
        _dense = newDense;
    }
}