using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Common.Utilities.Collections;

public ref struct RefEnumerator<T> where T : struct, IIdentifiable {
    public static RefEnumerator<T> Empty => new();
    
    private readonly SparseSet<T> _set;
    private readonly int[] _buffer;
    private readonly int _count;
    private int _index;

    public RefEnumerator(SparseSet<T> set, int[] buffer, int count) {
        _set = set;
        _buffer = buffer;
        _count = count;
        _index = -1;
    }

    public bool MoveNext() => ++_index < _count;
    public ref T Current => ref _set.Get(_buffer[_index]);
    public RefEnumerator<T> GetEnumerator() => this;
    
    public void Dispose() {
        if (_buffer != null)
            ArrayPool<int>.Shared.Return(_buffer);
    }
}