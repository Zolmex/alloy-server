using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities.Collections;

public class PooledList<T> : IEnumerable<T>, IDisposable {

    public int Count { get; private set; }

    private T[] _arr;
    
    public PooledList(int capacity = 10) {
        _arr = ArrayPool<T>.Shared.Rent(capacity);
    }

    public int Add(T elem, bool checkForDuplicate = false) {
        if (checkForDuplicate && _arr.IndexOf(elem) != -1)
            return -1;
        
        if (Count >= _arr.Length) {
            var newArr = ArrayPool<T>.Shared.Rent(Count * 2);
            _arr.AsSpan().CopyTo(newArr);
            ArrayPool<T>.Shared.Return(_arr);
            _arr = newArr;
        }

        _arr[Count++] = elem;
        return Count - 1;
    }

    public bool Remove(T elem) {
        int index = Array.IndexOf(_arr, elem, 0, Count);
        if (index < 0)
            return false;

        // Shift elements left to fill the gap
        _arr.AsSpan(index + 1, Count - index - 1).CopyTo(_arr.AsSpan(index));
    
        // Clear the vacated slot to avoid holding a reference (important for GC)
        if (!typeof(T).IsValueType)
            _arr[Count - 1] = default!;
    
        Count--;
        return true;
    }
    
    public bool RemoveAt(int index) {
        if (index < 0 || index >= Count)
            return false;

        // Shift elements left to fill the gap
        _arr.AsSpan(index + 1, Count - index - 1).CopyTo(_arr.AsSpan(index));
    
        // Clear the vacated slot to avoid holding a reference (important for GC)
        if (!typeof(T).IsValueType)
            _arr[Count - 1] = default!;
    
        Count--;
        return true;
    }

    public int IndexOf(T elem) {
        return Array.IndexOf(_arr, elem, 0, Count);
    }
    
    public T GetAt(int index) {
        if (index < 0 || index >= Count)
            return default;
        
        return _arr[index];
    }

    public void Clear() {
        Array.Clear(_arr);
        Count = 0;
    }

    public void Reset() { // Doesn't reset array values, it only resets the count
        Count = 0;
    }

    public bool Contains(T elem) {
        return Array.IndexOf(_arr, elem, 0, Count) != -1;
    }

    public Span<T> AsSpan() {
        return _arr.AsSpan(0, Count);
    }
    
    public IEnumerator<T> GetEnumerator() {
        // Iterate only over the live portion of the rented array
        for (int i = 0; i < Count; i++)
            yield return _arr[i];
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
    
    public void Dispose() {
        if (_arr is null)
            return;
        
        ArrayPool<T>.Shared.Return(_arr, clearArray: !typeof(T).IsValueType);
        _arr = null!;
        Count = 0;
    }
}