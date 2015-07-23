using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Facts
{    
    [Serializable]
    public class Fact<T1, T2>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly SafeSet<Tuple<T1, T2>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly TupleEqualityComparer<T1, T2> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _data = new SafeSet<Tuple<T1, T2>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2)
        {
            var neo = new Tuple<T1, T2>(arg1, arg2);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2)
        {
            Tuple<T1, T2> bundle;
            Predicate<Tuple<T1, T2>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2), predicate, out bundle);
        }

        public Tuple<T1, T2> Read(T1 arg1, T2 arg2)
        {
            Tuple<T1, T2> found;
            Predicate<Tuple<T1, T2>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null)
        {
            T1 _arg1;
            T2 _arg2;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    return new[] {Read(_arg1, _arg2)};
                }
                else
                {
                    Predicate<Tuple<T1, T2>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                Predicate<Tuple<T1, T2>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                  ;
                return PrivateRead(_index2, _arg2, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null)
        {
            T1 _arg1;
            T2 _arg2;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    Remove(_arg1, _arg2);
                }
                else
                {
                    Predicate<Tuple<T1, T2>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                Predicate<Tuple<T1, T2>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                  ;
                PrivateRemove(_index2, _arg2, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2);
            Predicate<Tuple<T1, T2>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2);
            Tuple<T1, T2> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
            }
        }

        private IEnumerable<Tuple<T1, T2>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly SafeSet<Tuple<T1, T2, T3>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly TupleEqualityComparer<T1, T2, T3> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _data = new SafeSet<Tuple<T1, T2, T3>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3)
        {
            var neo = new Tuple<T1, T2, T3>(arg1, arg2, arg3);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3)
        {
            Tuple<T1, T2, T3> bundle;
            Predicate<Tuple<T1, T2, T3>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3), predicate, out bundle);
        }

        public Tuple<T1, T2, T3> Read(T1 arg1, T2 arg2, T3 arg3)
        {
            Tuple<T1, T2, T3> found;
            Predicate<Tuple<T1, T2, T3>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        return new[] {Read(_arg1, _arg2, _arg3)};
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                  ;
                return PrivateRead(_index3, _arg3, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        Remove(_arg1, _arg2, _arg3);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                Predicate<Tuple<T1, T2, T3>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                  ;
                PrivateRemove(_index3, _arg3, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3);
            Predicate<Tuple<T1, T2, T3>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3);
            Tuple<T1, T2, T3> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3, T4>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly IEqualityComparer<T4> _comparer4;
        private readonly SafeSet<Tuple<T1, T2, T3, T4>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly NullAwareDictionary<T4, List<int>> _index4;
        private readonly TupleEqualityComparer<T1, T2, T3, T4> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _comparer4 = EqualityComparer<T4>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3, T4>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _index4 = new NullAwareDictionary<T4, List<int>>(_comparer4);
            _data = new SafeSet<Tuple<T1, T2, T3, T4>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var neo = new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
                _index4.Add(arg4, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Tuple<T1, T2, T3, T4> bundle;
            Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4), predicate, out bundle);
        }

        public Tuple<T1, T2, T3, T4> Read(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Tuple<T1, T2, T3, T4> found;
            Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3, T4>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            return new[] {Read(_arg1, _arg2, _arg3, _arg4)};
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              ;
                            return PrivateRead(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                  ;
                return PrivateRead(_index4, _arg4, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            Remove(_arg1, _arg2, _arg3, _arg4);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              ;
                            PrivateRemove(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                  ;
                PrivateRemove(_index4, _arg4, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3, arg4);
            Predicate<Tuple<T1, T2, T3, T4>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4);
            Tuple<T1, T2, T3, T4> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
                _index4.Remove(arg4, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private bool Check4(Needle<T4> arg, T4 obj)
        {
            T4 tmp;
            return !arg.TryGetValue(out tmp) || _comparer4.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3, T4>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3, T4, T5>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly IEqualityComparer<T4> _comparer4;
        private readonly IEqualityComparer<T5> _comparer5;
        private readonly SafeSet<Tuple<T1, T2, T3, T4, T5>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly NullAwareDictionary<T4, List<int>> _index4;
        private readonly NullAwareDictionary<T5, List<int>> _index5;
        private readonly TupleEqualityComparer<T1, T2, T3, T4, T5> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _comparer4 = EqualityComparer<T4>.Default;
            _comparer5 = EqualityComparer<T5>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3, T4, T5>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _index4 = new NullAwareDictionary<T4, List<int>>(_comparer4);
            _index5 = new NullAwareDictionary<T5, List<int>>(_comparer5);
            _data = new SafeSet<Tuple<T1, T2, T3, T4, T5>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var neo = new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
                _index4.Add(arg4, hash);
                _index5.Add(arg5, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Tuple<T1, T2, T3, T4, T5> bundle;
            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5), predicate, out bundle);
        }

        public Tuple<T1, T2, T3, T4, T5> Read(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Tuple<T1, T2, T3, T4, T5> found;
            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3, T4, T5>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                return new[] {Read(_arg1, _arg2, _arg3, _arg4, _arg5)};
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  ;
                                return PrivateRead(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              ;
                            return PrivateRead(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              ;
                            return PrivateRead(_index2, _arg2, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              ;
                            return PrivateRead(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          ;
                        return PrivateRead(_index3, _arg3, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          ;
                        return PrivateRead(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                      ;
                    return PrivateRead(_index4, _arg4, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      ;
                    return PrivateRead(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                  ;
                return PrivateRead(_index5, _arg5, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                Remove(_arg1, _arg2, _arg3, _arg4, _arg5);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  ;
                                PrivateRemove(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              ;
                            PrivateRemove(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              ;
                            PrivateRemove(_index2, _arg2, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              ;
                            PrivateRemove(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          ;
                        PrivateRemove(_index3, _arg3, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          ;
                        PrivateRemove(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                      ;
                    PrivateRemove(_index4, _arg4, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      ;
                    PrivateRemove(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                  ;
                PrivateRemove(_index5, _arg5, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5);
            Predicate<Tuple<T1, T2, T3, T4, T5>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5);
            Tuple<T1, T2, T3, T4, T5> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
                _index4.Remove(arg4, hash);
                _index5.Remove(arg5, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private bool Check4(Needle<T4> arg, T4 obj)
        {
            T4 tmp;
            return !arg.TryGetValue(out tmp) || _comparer4.Equals(tmp, obj);
        }

        private bool Check5(Needle<T5> arg, T5 obj)
        {
            T5 tmp;
            return !arg.TryGetValue(out tmp) || _comparer5.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3, T4, T5>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3, T4, T5, T6>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly IEqualityComparer<T4> _comparer4;
        private readonly IEqualityComparer<T5> _comparer5;
        private readonly IEqualityComparer<T6> _comparer6;
        private readonly SafeSet<Tuple<T1, T2, T3, T4, T5, T6>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly NullAwareDictionary<T4, List<int>> _index4;
        private readonly NullAwareDictionary<T5, List<int>> _index5;
        private readonly NullAwareDictionary<T6, List<int>> _index6;
        private readonly TupleEqualityComparer<T1, T2, T3, T4, T5, T6> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _comparer4 = EqualityComparer<T4>.Default;
            _comparer5 = EqualityComparer<T5>.Default;
            _comparer6 = EqualityComparer<T6>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3, T4, T5, T6>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _index4 = new NullAwareDictionary<T4, List<int>>(_comparer4);
            _index5 = new NullAwareDictionary<T5, List<int>>(_comparer5);
            _index6 = new NullAwareDictionary<T6, List<int>>(_comparer6);
            _data = new SafeSet<Tuple<T1, T2, T3, T4, T5, T6>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var neo = new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
                _index4.Add(arg4, hash);
                _index5.Add(arg5, hash);
                _index6.Add(arg6, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> bundle;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6), predicate, out bundle);
        }

        public Tuple<T1, T2, T3, T4, T5, T6> Read(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> found;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    return new[] {Read(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6)};
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      ;
                                    return PrivateRead(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  ;
                                return PrivateRead(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              ;
                            return PrivateRead(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  ;
                                return PrivateRead(_index2, _arg2, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  ;
                                return PrivateRead(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              ;
                            return PrivateRead(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              ;
                            return PrivateRead(_index3, _arg3, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              ;
                            return PrivateRead(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        return PrivateRead(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          ;
                        return PrivateRead(_index4, _arg4, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          ;
                        return PrivateRead(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    return PrivateRead(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                      ;
                    return PrivateRead(_index5, _arg5, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      ;
                    return PrivateRead(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                  ;
                return PrivateRead(_index6, _arg6, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    Remove(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6);
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      ;
                                    PrivateRemove(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  ;
                                PrivateRemove(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              ;
                            PrivateRemove(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  ;
                                PrivateRemove(_index2, _arg2, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  ;
                                PrivateRemove(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              ;
                            PrivateRemove(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              ;
                            PrivateRemove(_index3, _arg3, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              ;
                            PrivateRemove(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          ;
                        PrivateRemove(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          ;
                        PrivateRemove(_index4, _arg4, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          ;
                        PrivateRemove(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      ;
                    PrivateRemove(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                      ;
                    PrivateRemove(_index5, _arg5, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      ;
                    PrivateRemove(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                  ;
                PrivateRemove(_index6, _arg6, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6);
            Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6);
            Tuple<T1, T2, T3, T4, T5, T6> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
                _index4.Remove(arg4, hash);
                _index5.Remove(arg5, hash);
                _index6.Remove(arg6, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private bool Check4(Needle<T4> arg, T4 obj)
        {
            T4 tmp;
            return !arg.TryGetValue(out tmp) || _comparer4.Equals(tmp, obj);
        }

        private bool Check5(Needle<T5> arg, T5 obj)
        {
            T5 tmp;
            return !arg.TryGetValue(out tmp) || _comparer5.Equals(tmp, obj);
        }

        private bool Check6(Needle<T6> arg, T6 obj)
        {
            T6 tmp;
            return !arg.TryGetValue(out tmp) || _comparer6.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3, T4, T5, T6, T7>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly IEqualityComparer<T4> _comparer4;
        private readonly IEqualityComparer<T5> _comparer5;
        private readonly IEqualityComparer<T6> _comparer6;
        private readonly IEqualityComparer<T7> _comparer7;
        private readonly SafeSet<Tuple<T1, T2, T3, T4, T5, T6, T7>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly NullAwareDictionary<T4, List<int>> _index4;
        private readonly NullAwareDictionary<T5, List<int>> _index5;
        private readonly NullAwareDictionary<T6, List<int>> _index6;
        private readonly NullAwareDictionary<T7, List<int>> _index7;
        private readonly TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _comparer4 = EqualityComparer<T4>.Default;
            _comparer5 = EqualityComparer<T5>.Default;
            _comparer6 = EqualityComparer<T6>.Default;
            _comparer7 = EqualityComparer<T7>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _index4 = new NullAwareDictionary<T4, List<int>>(_comparer4);
            _index5 = new NullAwareDictionary<T5, List<int>>(_comparer5);
            _index6 = new NullAwareDictionary<T6, List<int>>(_comparer6);
            _index7 = new NullAwareDictionary<T7, List<int>>(_comparer7);
            _data = new SafeSet<Tuple<T1, T2, T3, T4, T5, T6, T7>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var neo = new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
                _index4.Add(arg4, hash);
                _index5.Add(arg5, hash);
                _index6.Add(arg6, hash);
                _index7.Add(arg7, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> bundle;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7), predicate, out bundle);
        }

        public Tuple<T1, T2, T3, T4, T5, T6, T7> Read(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> found;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null, Needle<T7> arg7 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            T7 _arg7;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    if (arg7.TryGetValue(out _arg7))
                                    {
                                        return new[] {Read(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7)};
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          ;
                                        return PrivateRead(_index1, _arg1, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && Check7(arg7, tuple.Item7)
                                                                                      ;
                                    return PrivateRead(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  ;
                                return PrivateRead(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            return PrivateRead(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                if (arg7.TryGetValue(out _arg7))
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      ;
                                    return PrivateRead(_index2, _arg2, predicate);
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      ;
                                    return PrivateRead(_index2, _arg2, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  ;
                                return PrivateRead(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            return PrivateRead(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            if (arg7.TryGetValue(out _arg7))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  ;
                                return PrivateRead(_index3, _arg3, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  ;
                                return PrivateRead(_index3, _arg3, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            return PrivateRead(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        return PrivateRead(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        if (arg7.TryGetValue(out _arg7))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              ;
                            return PrivateRead(_index4, _arg4, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              ;
                            return PrivateRead(_index4, _arg4, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        return PrivateRead(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    return PrivateRead(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    if (arg7.TryGetValue(out _arg7))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          ;
                        return PrivateRead(_index5, _arg5, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          ;
                        return PrivateRead(_index5, _arg5, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    return PrivateRead(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                if (arg7.TryGetValue(out _arg7))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                      ;
                    return PrivateRead(_index6, _arg6, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      ;
                    return PrivateRead(_index6, _arg6, predicate);
                }
            }
            else if (arg7.TryGetValue(out _arg7))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                  ;
                return PrivateRead(_index7, _arg7, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null, Needle<T7> arg7 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            T7 _arg7;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    if (arg7.TryGetValue(out _arg7))
                                    {
                                        Remove(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7);
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          ;
                                        PrivateRemove(_index1, _arg1, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && Check7(arg7, tuple.Item7)
                                                                                      ;
                                    PrivateRemove(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  ;
                                PrivateRemove(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            PrivateRemove(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                if (arg7.TryGetValue(out _arg7))
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      ;
                                    PrivateRemove(_index2, _arg2, predicate);
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      ;
                                    PrivateRemove(_index2, _arg2, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  ;
                                PrivateRemove(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            PrivateRemove(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            if (arg7.TryGetValue(out _arg7))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  ;
                                PrivateRemove(_index3, _arg3, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  ;
                                PrivateRemove(_index3, _arg3, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              ;
                            PrivateRemove(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        PrivateRemove(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        if (arg7.TryGetValue(out _arg7))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              ;
                            PrivateRemove(_index4, _arg4, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              ;
                            PrivateRemove(_index4, _arg4, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          ;
                        PrivateRemove(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    PrivateRemove(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    if (arg7.TryGetValue(out _arg7))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          ;
                        PrivateRemove(_index5, _arg5, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          ;
                        PrivateRemove(_index5, _arg5, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      ;
                    PrivateRemove(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                if (arg7.TryGetValue(out _arg7))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                      ;
                    PrivateRemove(_index6, _arg6, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      ;
                    PrivateRemove(_index6, _arg6, predicate);
                }
            }
            else if (arg7.TryGetValue(out _arg7))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                  ;
                PrivateRemove(_index7, _arg7, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Tuple<T1, T2, T3, T4, T5, T6, T7> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
                _index4.Remove(arg4, hash);
                _index5.Remove(arg5, hash);
                _index6.Remove(arg6, hash);
                _index7.Remove(arg7, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private bool Check4(Needle<T4> arg, T4 obj)
        {
            T4 tmp;
            return !arg.TryGetValue(out tmp) || _comparer4.Equals(tmp, obj);
        }

        private bool Check5(Needle<T5> arg, T5 obj)
        {
            T5 tmp;
            return !arg.TryGetValue(out tmp) || _comparer5.Equals(tmp, obj);
        }

        private bool Check6(Needle<T6> arg, T6 obj)
        {
            T6 tmp;
            return !arg.TryGetValue(out tmp) || _comparer6.Equals(tmp, obj);
        }

        private bool Check7(Needle<T7> arg, T7 obj)
        {
            T7 tmp;
            return !arg.TryGetValue(out tmp) || _comparer7.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6, T7> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6, T7> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
    
    [Serializable]
    public class Fact<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private readonly IEqualityComparer<T1> _comparer1;
        private readonly IEqualityComparer<T2> _comparer2;
        private readonly IEqualityComparer<T3> _comparer3;
        private readonly IEqualityComparer<T4> _comparer4;
        private readonly IEqualityComparer<T5> _comparer5;
        private readonly IEqualityComparer<T6> _comparer6;
        private readonly IEqualityComparer<T7> _comparer7;
        private readonly IEqualityComparer<T8> _comparer8;
        private readonly SafeSet<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> _data;
        private readonly NullAwareDictionary<T1, List<int>> _index1;
        private readonly NullAwareDictionary<T2, List<int>> _index2;
        private readonly NullAwareDictionary<T3, List<int>> _index3;
        private readonly NullAwareDictionary<T4, List<int>> _index4;
        private readonly NullAwareDictionary<T5, List<int>> _index5;
        private readonly NullAwareDictionary<T6, List<int>> _index6;
        private readonly NullAwareDictionary<T7, List<int>> _index7;
        private readonly NullAwareDictionary<T8, List<int>> _index8;
        private readonly TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8> _tupleComparer;

        public Fact()
        {
            _comparer1 = EqualityComparer<T1>.Default;
            _comparer2 = EqualityComparer<T2>.Default;
            _comparer3 = EqualityComparer<T3>.Default;
            _comparer4 = EqualityComparer<T4>.Default;
            _comparer5 = EqualityComparer<T5>.Default;
            _comparer6 = EqualityComparer<T6>.Default;
            _comparer7 = EqualityComparer<T7>.Default;
            _comparer8 = EqualityComparer<T8>.Default;
            _tupleComparer = TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8>.Default;
            _index1 = new NullAwareDictionary<T1, List<int>>(_comparer1);
            _index2 = new NullAwareDictionary<T2, List<int>>(_comparer2);
            _index3 = new NullAwareDictionary<T3, List<int>>(_comparer3);
            _index4 = new NullAwareDictionary<T4, List<int>>(_comparer4);
            _index5 = new NullAwareDictionary<T5, List<int>>(_comparer5);
            _index6 = new NullAwareDictionary<T6, List<int>>(_comparer6);
            _index7 = new NullAwareDictionary<T7, List<int>>(_comparer7);
            _index8 = new NullAwareDictionary<T8, List<int>>(_comparer8);
            _data = new SafeSet<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>(_tupleComparer);
        }

        public void Add(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var neo = new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            var hash = _tupleComparer.GetHashCode(neo);
            if (_data.Add(neo))
            {
                _index1.Add(arg1, hash);
                _index2.Add(arg2, hash);
                _index3.Add(arg3, hash);
                _index4.Add(arg4, hash);
                _index5.Add(arg5, hash);
                _index6.Add(arg6, hash);
                _index7.Add(arg7, hash);
                _index8.Add(arg8, hash);
            }
        }

        public bool Query(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, T8> bundle;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return _data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), predicate, out bundle);
        }

        public Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Read(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, T8> found;
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            if (_data.TryGetValue(_tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), predicate, out found))
            {
                return found;
            }
            return null;
        }

        public IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> Read(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null, Needle<T7> arg7 = null, Needle<T8> arg8 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            T7 _arg7;
            T8 _arg8;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    if (arg7.TryGetValue(out _arg7))
                                    {
                                        if (arg8.TryGetValue(out _arg8))
                                        {
                                            return new[] {Read(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8)};
                                        }
                                        else
                                        {
                                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                              ;
                                            return PrivateRead(_index1, _arg1, predicate);
                                        }
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && Check8(arg8, tuple.Rest)
                                                                                          ;
                                        return PrivateRead(_index1, _arg1, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && Check7(arg7, tuple.Item7)
                                                                                      && Check8(arg8, tuple.Rest)
                                                                                      ;
                                    return PrivateRead(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                return PrivateRead(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            return PrivateRead(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        return PrivateRead(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                if (arg7.TryGetValue(out _arg7))
                                {
                                    if (arg8.TryGetValue(out _arg8))
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                          && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                          ;
                                        return PrivateRead(_index2, _arg2, predicate);
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                          ;
                                        return PrivateRead(_index2, _arg2, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && Check8(arg8, tuple.Rest)
                                                                                      ;
                                    return PrivateRead(_index2, _arg2, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                return PrivateRead(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            return PrivateRead(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        return PrivateRead(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            if (arg7.TryGetValue(out _arg7))
                            {
                                if (arg8.TryGetValue(out _arg8))
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                      ;
                                    return PrivateRead(_index3, _arg3, predicate);
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      ;
                                    return PrivateRead(_index3, _arg3, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                return PrivateRead(_index3, _arg3, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            return PrivateRead(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        return PrivateRead(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        if (arg7.TryGetValue(out _arg7))
                        {
                            if (arg8.TryGetValue(out _arg8))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                  ;
                                return PrivateRead(_index4, _arg4, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  ;
                                return PrivateRead(_index4, _arg4, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            return PrivateRead(_index4, _arg4, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        return PrivateRead(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    if (arg7.TryGetValue(out _arg7))
                    {
                        if (arg8.TryGetValue(out _arg8))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              && _comparer8.Equals(tuple.Rest, _arg8)
                                                                              ;
                            return PrivateRead(_index5, _arg5, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              ;
                            return PrivateRead(_index5, _arg5, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        return PrivateRead(_index5, _arg5, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                if (arg7.TryGetValue(out _arg7))
                {
                    if (arg8.TryGetValue(out _arg8))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          && _comparer8.Equals(tuple.Rest, _arg8)
                                                                          ;
                        return PrivateRead(_index6, _arg6, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          ;
                        return PrivateRead(_index6, _arg6, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    return PrivateRead(_index6, _arg6, predicate);
                }
            }
            else if (arg7.TryGetValue(out _arg7))
            {
                if (arg8.TryGetValue(out _arg8))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                      && _comparer8.Equals(tuple.Rest, _arg8)
                                                                      ;
                    return PrivateRead(_index7, _arg7, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                      ;
                    return PrivateRead(_index7, _arg7, predicate);
                }
            }
            else if (arg8.TryGetValue(out _arg8))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer8.Equals(tuple.Rest, _arg8)
                                                                  ;
                return PrivateRead(_index8, _arg8, predicate);
            }
            throw new ArgumentException();
        }

        public void Remove(Needle<T1> arg1 = null, Needle<T2> arg2 = null, Needle<T3> arg3 = null, Needle<T4> arg4 = null, Needle<T5> arg5 = null, Needle<T6> arg6 = null, Needle<T7> arg7 = null, Needle<T8> arg8 = null)
        {
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            T4 _arg4;
            T5 _arg5;
            T6 _arg6;
            T7 _arg7;
            T8 _arg8;
            if (arg1.TryGetValue(out _arg1))
            {
                if (arg2.TryGetValue(out _arg2))
                {
                    if (arg3.TryGetValue(out _arg3))
                    {
                        if (arg4.TryGetValue(out _arg4))
                        {
                            if (arg5.TryGetValue(out _arg5))
                            {
                                if (arg6.TryGetValue(out _arg6))
                                {
                                    if (arg7.TryGetValue(out _arg7))
                                    {
                                        if (arg8.TryGetValue(out _arg8))
                                        {
                                            Remove(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8);
                                        }
                                        else
                                        {
                                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                              ;
                                            PrivateRemove(_index1, _arg1, predicate);
                                        }
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && Check8(arg8, tuple.Rest)
                                                                                          ;
                                        PrivateRemove(_index1, _arg1, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                      && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && Check7(arg7, tuple.Item7)
                                                                                      && Check8(arg8, tuple.Rest)
                                                                                      ;
                                    PrivateRemove(_index1, _arg1, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                                  && _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && Check6(arg6, tuple.Item6)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                PrivateRemove(_index1, _arg1, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                              && _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && Check5(arg5, tuple.Item5)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            PrivateRemove(_index1, _arg1, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                          && _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && Check4(arg4, tuple.Item4)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        PrivateRemove(_index1, _arg1, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer1.Equals(tuple.Item1, _arg1)
                                                                      && Check3(arg3, tuple.Item3)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index1, _arg1, predicate);
                }
            }
            else if (arg2.TryGetValue(out _arg2))
            {
                if (arg3.TryGetValue(out _arg3))
                {
                    if (arg4.TryGetValue(out _arg4))
                    {
                        if (arg5.TryGetValue(out _arg5))
                        {
                            if (arg6.TryGetValue(out _arg6))
                            {
                                if (arg7.TryGetValue(out _arg7))
                                {
                                    if (arg8.TryGetValue(out _arg8))
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                          && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                          ;
                                        PrivateRemove(_index2, _arg2, predicate);
                                    }
                                    else
                                    {
                                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                          ;
                                        PrivateRemove(_index2, _arg2, predicate);
                                    }
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                      && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && Check8(arg8, tuple.Rest)
                                                                                      ;
                                    PrivateRemove(_index2, _arg2, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                                  && _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && Check7(arg7, tuple.Item7)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                PrivateRemove(_index2, _arg2, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                              && _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && Check6(arg6, tuple.Item6)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            PrivateRemove(_index2, _arg2, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                          && _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && Check5(arg5, tuple.Item5)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        PrivateRemove(_index2, _arg2, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer2.Equals(tuple.Item2, _arg2)
                                                                      && Check4(arg4, tuple.Item4)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index2, _arg2, predicate);
                }
            }
            else if (arg3.TryGetValue(out _arg3))
            {
                if (arg4.TryGetValue(out _arg4))
                {
                    if (arg5.TryGetValue(out _arg5))
                    {
                        if (arg6.TryGetValue(out _arg6))
                        {
                            if (arg7.TryGetValue(out _arg7))
                            {
                                if (arg8.TryGetValue(out _arg8))
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                      ;
                                    PrivateRemove(_index3, _arg3, predicate);
                                }
                                else
                                {
                                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                      && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                      && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                      && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                      && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                      ;
                                    PrivateRemove(_index3, _arg3, predicate);
                                }
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                                  && _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && Check8(arg8, tuple.Rest)
                                                                                  ;
                                PrivateRemove(_index3, _arg3, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                              && _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && Check7(arg7, tuple.Item7)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            PrivateRemove(_index3, _arg3, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                          && _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && Check6(arg6, tuple.Item6)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        PrivateRemove(_index3, _arg3, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer3.Equals(tuple.Item3, _arg3)
                                                                      && Check5(arg5, tuple.Item5)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index3, _arg3, predicate);
                }
            }
            else if (arg4.TryGetValue(out _arg4))
            {
                if (arg5.TryGetValue(out _arg5))
                {
                    if (arg6.TryGetValue(out _arg6))
                    {
                        if (arg7.TryGetValue(out _arg7))
                        {
                            if (arg8.TryGetValue(out _arg8))
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  && _comparer8.Equals(tuple.Rest, _arg8)
                                                                                  ;
                                PrivateRemove(_index4, _arg4, predicate);
                            }
                            else
                            {
                                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                                  && _comparer5.Equals(tuple.Item5, _arg5)
                                                                                  && _comparer6.Equals(tuple.Item6, _arg6)
                                                                                  && _comparer7.Equals(tuple.Item7, _arg7)
                                                                                  ;
                                PrivateRemove(_index4, _arg4, predicate);
                            }
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                              && _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && Check8(arg8, tuple.Rest)
                                                                              ;
                            PrivateRemove(_index4, _arg4, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                          && _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && Check7(arg7, tuple.Item7)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        PrivateRemove(_index4, _arg4, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer4.Equals(tuple.Item4, _arg4)
                                                                      && Check6(arg6, tuple.Item6)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index4, _arg4, predicate);
                }
            }
            else if (arg5.TryGetValue(out _arg5))
            {
                if (arg6.TryGetValue(out _arg6))
                {
                    if (arg7.TryGetValue(out _arg7))
                    {
                        if (arg8.TryGetValue(out _arg8))
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              && _comparer8.Equals(tuple.Rest, _arg8)
                                                                              ;
                            PrivateRemove(_index5, _arg5, predicate);
                        }
                        else
                        {
                            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                              && _comparer6.Equals(tuple.Item6, _arg6)
                                                                              && _comparer7.Equals(tuple.Item7, _arg7)
                                                                              ;
                            PrivateRemove(_index5, _arg5, predicate);
                        }
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                          && _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && Check8(arg8, tuple.Rest)
                                                                          ;
                        PrivateRemove(_index5, _arg5, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer5.Equals(tuple.Item5, _arg5)
                                                                      && Check7(arg7, tuple.Item7)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index5, _arg5, predicate);
                }
            }
            else if (arg6.TryGetValue(out _arg6))
            {
                if (arg7.TryGetValue(out _arg7))
                {
                    if (arg8.TryGetValue(out _arg8))
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          && _comparer8.Equals(tuple.Rest, _arg8)
                                                                          ;
                        PrivateRemove(_index6, _arg6, predicate);
                    }
                    else
                    {
                        Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                          && _comparer7.Equals(tuple.Item7, _arg7)
                                                                          ;
                        PrivateRemove(_index6, _arg6, predicate);
                    }
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer6.Equals(tuple.Item6, _arg6)
                                                                      && Check8(arg8, tuple.Rest)
                                                                      ;
                    PrivateRemove(_index6, _arg6, predicate);
                }
            }
            else if (arg7.TryGetValue(out _arg7))
            {
                if (arg8.TryGetValue(out _arg8))
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                      && _comparer8.Equals(tuple.Rest, _arg8)
                                                                      ;
                    PrivateRemove(_index7, _arg7, predicate);
                }
                else
                {
                    Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer7.Equals(tuple.Item7, _arg7)
                                                                      ;
                    PrivateRemove(_index7, _arg7, predicate);
                }
            }
            else if (arg8.TryGetValue(out _arg8))
            {
                Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _comparer8.Equals(tuple.Rest, _arg8)
                                                                  ;
                PrivateRemove(_index8, _arg8, predicate);
            }
        }

        public void Remove(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var hash = _tupleComparer.GetHashCode(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate = tuple => _tupleComparer.Equals(tuple, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Tuple<T1, T2, T3, T4, T5, T6, T7, T8> removed;
            if (_data.Remove(hash, predicate, out removed))
            {
                _index1.Remove(arg1, hash);
                _index2.Remove(arg2, hash);
                _index3.Remove(arg3, hash);
                _index4.Remove(arg4, hash);
                _index5.Remove(arg5, hash);
                _index6.Remove(arg6, hash);
                _index7.Remove(arg7, hash);
                _index8.Remove(arg8, hash);
            }
        }

        private bool Check3(Needle<T3> arg, T3 obj)
        {
            T3 tmp;
            return !arg.TryGetValue(out tmp) || _comparer3.Equals(tmp, obj);
        }

        private bool Check4(Needle<T4> arg, T4 obj)
        {
            T4 tmp;
            return !arg.TryGetValue(out tmp) || _comparer4.Equals(tmp, obj);
        }

        private bool Check5(Needle<T5> arg, T5 obj)
        {
            T5 tmp;
            return !arg.TryGetValue(out tmp) || _comparer5.Equals(tmp, obj);
        }

        private bool Check6(Needle<T6> arg, T6 obj)
        {
            T6 tmp;
            return !arg.TryGetValue(out tmp) || _comparer6.Equals(tmp, obj);
        }

        private bool Check7(Needle<T7> arg, T7 obj)
        {
            T7 tmp;
            return !arg.TryGetValue(out tmp) || _comparer7.Equals(tmp, obj);
        }

        private bool Check8(Needle<T8> arg, T8 obj)
        {
            T8 tmp;
            return !arg.TryGetValue(out tmp) || _comparer8.Equals(tmp, obj);
        }

        private IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> PrivateRead<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6, T7, T8> found;
                    if (_data.TryGetValue(hash, predicate, out found))
                    {
                        yield return found;
                    }
                }
            }
        }

        private void PrivateRemove<T>(NullAwareDictionary<T, List<int>> index, T arg, Predicate<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> predicate)
        {
            if (index.ContainsKey(arg))
            {
                foreach (var hash in index[arg])
                {
                    Tuple<T1, T2, T3, T4, T5, T6, T7, T8> found;
                    _data.Remove(hash, predicate, out found);
                }
            }
        }
}
}