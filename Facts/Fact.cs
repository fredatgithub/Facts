using System;
using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Facts
{
    [Serializable]
    public class Fact<T>
    {
        private readonly SafeSet<Tuple<T>> _data;
        private readonly TupleEqualityComparer<T> _tupleComparer;

        public Fact()
        {
            _tupleComparer = TupleEqualityComparer<T>.Default;
            _data = new SafeSet<Tuple<T>>(_tupleComparer);
        }

        public void Add(T obj)
        {
            var neo = new Tuple<T>(obj);
            _data.Add(neo);
        }

        public bool Query(T obj)
        {
            Tuple<T> bundle;
            Predicate<Tuple<T>> predicate = tuple => _tupleComparer.Equals(tuple, obj);
            return _data.TryGetValue(_tupleComparer.GetHashCode(obj), predicate, out bundle);
        }

        public Tuple<T> Read(T obj)
        {
            Tuple<T> found;
            Predicate<Tuple<T>> predicate = tuple => _tupleComparer.Equals(tuple, obj);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(obj), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T>> Read(Needle<T> obj = null)
        {
            T _obj;
            if (obj.TryGetValue(out _obj))
            {
                return new[] { Read(_obj) };
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T> obj = null)
        {
            T _obj;
            if (obj.TryGetValue(out _obj))
            {
                Remove(_obj);
            }
        }

        public void Remove(T obj)
        {
            var hash = _tupleComparer.GetHashCode(obj);
            Predicate<Tuple<T>> predicate = tuple => _tupleComparer.Equals(tuple, obj);
            Tuple<T> removed;
            _data.Remove(hash, predicate, out removed);
        }
    }
}