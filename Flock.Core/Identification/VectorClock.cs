using System.Collections.Generic;
using System.Linq;

namespace Flock.Core.Identification {
    
    public class VectorClock<TKey> {
        
        #region Enums

        public enum VectorComparision {
            Greater,    // if one > other
            Equal,      // if one == other
            Smaller,    // if one < other
            Concurrent  // if one <> other
        }

        #endregion

        private const int DefaultValue = 0;

        #region State

        public Dictionary<TKey, int> Map { get; private set; }

        #endregion

        #region Properties

        public IEnumerable<TKey> Keys {
            get {
                return Map.Keys;
            }
        }

        #endregion

        #region Constructors

        public VectorClock() {
            Map = new Dictionary<TKey, int>();
        }

        public VectorClock(VectorClock<TKey> other) : this(other.Map) { }

        private VectorClock(Dictionary<TKey, int> dictionary) {
            Map = dictionary.ToDictionary(e => e.Key, e => e.Value);
        }

        #endregion

        #region Public Methods

        public void Tick(TKey key) {
            if (Map.TryGetValue(key, out int value)) {
                Map[key] = value + 1;
            } else {
                Map.Add(key, 1);
            }
        }

        public int GetValue(TKey key) {
            return Map.TryGetValue(key, out int value) ? value : DefaultValue;
        }

        public bool Contains(TKey key) {
            return Map.ContainsKey(key);
        }

        private bool IsDefaultValue(TKey key) {
            return GetValue(key) == DefaultValue;
        }

        public void Merge(VectorClock<TKey> other) {
            var temp = new VectorClock<TKey>(Map);

            foreach (var k in other.Keys) {
                if (!temp.Contains(k) || temp.GetValue(k) < other.GetValue(k)) {
                    temp.Put(k, other.GetValue(k));
                }
            }

            Map = temp.Map;
        }

        #endregion

        #region Static

        public static VectorComparision Compare(VectorClock<TKey> one, VectorClock<TKey> other) {

            var equal = true;
            var greater = true;
            var smaller = true;

            foreach (var k in one.Keys) {
                var oneValue = one.GetValue(k);

                if (other.Contains(k)) {
                    var otherValue = other.GetValue(k);

                    if (oneValue < otherValue) {
                        equal = false;
                        greater = false;
                    }

                    if (oneValue > otherValue) {
                        equal = false;
                        smaller = false;
                    }
                } else if (oneValue != DefaultValue) {
                    equal = false;
                    smaller = false;
                }
            }

            foreach (var k in other.Keys) {
                if (!one.Contains(k) && !other.IsDefaultValue(k)) {
                    equal = false;
                    greater = false;
                }
            }

            if (equal)
                return VectorComparision.Equal;
            if (greater)
                return VectorComparision.Greater;
            if (smaller)
                return VectorComparision.Smaller;
            else
                return VectorComparision.Concurrent;
        }

        public static VectorClock<TKey> Merge(VectorClock<TKey> one, VectorClock<TKey> other) {
            var result = new VectorClock<TKey>(one);
            result.Merge(other);
            return result;
        }

        #endregion

    }

    #region Extension Methods

    public static class VectorClockExtensions {
        
        public static void Put<TKey>(this VectorClock<TKey> vectorClock, TKey key, int value) {
            Put(vectorClock.Map, key, value);
        }

        public static void Put<TKey>(this Dictionary<TKey, int> map, TKey key, int value) {
            if (!map.ContainsKey(key))
                map.Add(key, value);
            else
                map[key] = value;
        }

    }

    #endregion

}
