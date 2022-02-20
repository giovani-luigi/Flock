using System;
using System.Collections.Generic;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Counters {

    /// <summary>
    /// Grow only counter (G-Counter)
    /// </summary>
    /// <remarks>
    /// A G-Counter is a grow-only counter Conflict-free Replicated Data Type (CRDT) in which only increment and merge are possible.
    /// Incrementing the counter adds N to the count for the current replica.
    /// Conflicts are resolved by taking the maximum count for each replica.
    /// The value of the counter is the sum of all replica counts.
    /// </remarks>
    /// <typeparam name="TKey">
    /// The key used to associate with a particular counter.
    /// </typeparam>
    [MessagePackObject()]
    public sealed class GCounter<TKey> : ICrdt<GCounter<TKey>> {

        #region State
        
        [Key(0)]
        private readonly Dictionary<TKey, ulong> _counters;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="GCounter{TKey}"/> with initial count set to Zero.
        /// </summary>
        public GCounter() {
            _counters = new Dictionary<TKey, ulong>();
        }

        /// <summary>
        /// Creates a <see cref="GCounter{TKey}"/> with data copied from the specified <paramref name="counter"/>
        /// </summary>
        public GCounter(GCounter<TKey> counter) {
            _counters = new Dictionary<TKey, ulong>(counter._counters);
        }

        [SerializationConstructor]
        internal GCounter(Dictionary<TKey, ulong> counters) {
            _counters = counters;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increments the count for the given key
        /// </summary>
        public void Increment(TKey key, ulong increment = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (increment == 0) return; // handle politely
            if (_counters.TryGetValue(key, out ulong count)) {
                _counters[key] = count + increment;
            } else {
                _counters[key] = increment;
            }
        }

        /// <summary>
        /// Gets the total 
        /// </summary>
        public ulong GetValue() {
            ulong sum = 0;
            foreach (ulong count in _counters.Values) 
                sum += count;
            return sum;
        }

        #endregion
        
        #region Implementation of ICRDT<GCounter<TKey>>

        /// <inheritdoc />
        public void Merge(GCounter<TKey> other) {
            foreach (var kvp in other._counters) {
                if (_counters.TryGetValue(kvp.Key, out ulong thisCount)) {
                    _counters[kvp.Key] = Math.Max(thisCount, kvp.Value); // resolve conflict: greater count wins
                } else {
                    _counters.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// <inheritdoc />
        public GCounter<TKey> Copy() {
            return new GCounter<TKey>(this);
        }

        #endregion
        
        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(GCounter<TKey>)}, Value=" + GetValue();
        }

        #endregion
    }
}
