using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities.Collections;

public ref struct SparseEnumerator<T> where T : struct, IEntityIdentifiable {
    private readonly SparseSet<T> _set;
    private int _index = 0;

    public SparseEnumerator(SparseSet<T> set) {
        _set = set;
    }

    public bool MoveNext() => ++_index < _set.Count;

    public ref T Current => ref _set.GetAt(_index);
    
    public SparseEnumerator<T> GetEnumerator() {
        return this;
    }
}