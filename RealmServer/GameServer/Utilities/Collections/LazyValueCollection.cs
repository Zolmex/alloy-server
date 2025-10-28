#region

using Common.Utilities;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace GameServer.Utilities.Collections
{
    public class LazyValueCollection<T> : IEnumerable<T> where T : IIdentifiable
    {
        private readonly ConcurrentDictionary<int, T> _dict;

        public LazyValueCollection(ref ConcurrentDictionary<int, T> dict)
        {
            _dict = dict;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var dictEnum = _dict.Values.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var dictEnum = _dict.Values.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }
    }
    
    public class LazyValueCollection<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict;

        public LazyValueCollection(ref ConcurrentDictionary<TKey, TValue> dict)
        {
            _dict = dict;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            var dictEnum = _dict.Values.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var dictEnum = _dict.Values.GetEnumerator();
            while (dictEnum.MoveNext())
                yield return dictEnum.Current;
            dictEnum.Dispose();
        }
    }
}