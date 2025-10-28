#region

using Common.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace GameServer.Utilities.Collections
{
    public class LazyCollection<T> : IEnumerable<KeyValuePair<int, T>> where T : IIdentifiable
    {
        public int Count => _dict.Count;
        public LazyValueCollection<T> Values { get; }

        public event Action<T> OnAdd;
        public event Action<T> OnRemove;

        protected readonly ConcurrentDictionary<int, T> _dict = [];
        protected readonly ConcurrentQueue<(T, bool)> _pending = [];

        public LazyCollection()
        {
            Values = new LazyValueCollection<T>(ref _dict);
        }

        public void Update()
        {
            while (_pending.TryDequeue(out var elem))
            {
                if (elem.Item2)
                {
                    if (_dict.TryAdd(elem.Item1.Id, elem.Item1))
                        OnAdd?.Invoke(elem.Item1);
                }
                else
                {
                    if (_dict.Remove(elem.Item1.Id, out _))
                        OnRemove?.Invoke(elem.Item1);
                }
            }
        }

        public bool TryGetValue(int itemId, out T ret)
        {
            ret = default;
            if (!_dict.TryGetValue(itemId, out ret))
                return false;
            return true;
        }

        public void Add(T item)
        {
            _pending.Enqueue((item, true));
        }

        public void Remove(T item)
        {
            _pending.Enqueue((item, false));
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        public void Clear()
        {
            _pending.Clear();
            _dict.Clear();
            OnAdd = null;
            OnRemove = null;
        }
    }
    
    public class LazyCollection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public int Count => _dict.Count;
        public LazyValueCollection<TKey, TValue> Values { get; }

        public event Action<TValue> OnAdd;
        public event Action<TValue> OnRemove;

        protected readonly ConcurrentDictionary<TKey, TValue> _dict = [];
        protected readonly ConcurrentQueue<(TKey, TValue, bool)> _pending = [];

        public LazyCollection()
        {
            Values = new LazyValueCollection<TKey, TValue>(ref _dict);
        }

        public void Update()
        {
            while (_pending.TryDequeue(out var elem))
            {
                if (elem.Item3)
                {
                    if (_dict.TryAdd(elem.Item1, elem.Item2))
                        OnAdd?.Invoke(elem.Item2);
                }
                else
                {
                    if (_dict.Remove(elem.Item1, out _))
                        OnRemove?.Invoke(elem.Item2);
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue val)
        {
            val = default;
            if (!_dict.TryGetValue(key, out val))
                return false;
            return true;
        }

        public void Add(TKey key, TValue value)
        {
            _pending.Enqueue((key, value, true));
        }

        public void Remove(TKey key, TValue value)
        {
            _pending.Enqueue((key, value, false));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var dictEnum = _dict.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        public void Clear()
        {
            _pending.Clear();
            _dict.Clear();
            OnAdd = null;
            OnRemove = null;
        }
    }
}