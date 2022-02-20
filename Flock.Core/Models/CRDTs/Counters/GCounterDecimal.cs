using System;
using System.Collections.Generic;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Counters {

    /// <summary>
    /// Grow Only counter supporting decimal values (G-CounterDecimal)
    /// (by Giovani Luigi)
    /// </summary>
    /// <remarks>
    /// A Decimal G-Counter is a grow-only counter Conflict-free Replicated Data Type (CRDT)
    /// in which only increment and merge are possible. Incrementing the counter adds N to the
    /// count for the current replica. Conflicts are resolved by taking the maximum count for each replica.
    /// The value of the counter is the sum of all replica counts.
    /// </remarks>
    /// <typeparam name="TKey">
    /// The key used to associate with a particular counter.
    /// </typeparam>
    [MessagePackObject()]
    public sealed class GCounterDecimal<TKey> : ICrdt<GCounterDecimal<TKey>> {
        
        #region State

        [Key(0)]
        private readonly Dictionary<TKey, decimal> _counters;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="GCounterDecimal{TKey}"/> with initial count set to Zero.
        /// </summary>
        public GCounterDecimal() {
            _counters = new Dictionary<TKey, decimal>();
        }

        /// <summary>
        /// Creates a <see cref="GCounterDecimal{TKey}"/> with data copied from the specified <paramref name="counter"/>
        /// </summary>
        public GCounterDecimal(GCounterDecimal<TKey> counter) {
            _counters = new Dictionary<TKey, decimal>(counter._counters);
        }
        
        [SerializationConstructor]
        internal GCounterDecimal(Dictionary<TKey, decimal> counters) {
            _counters = counters;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increments the count for the given key
        /// </summary>
        public void Increment(TKey key, decimal increment = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (increment < 0) throw new ArgumentOutOfRangeException(nameof(increment), "This counter can't decrement");
            if (increment == 0) return; // handle politely
            if (_counters.TryGetValue(key, out decimal count)) {
                _counters[key] = count + increment;
            } else {
                _counters[key] = increment;
            }
        }

        /// <summary>
        /// Gets the total 
        /// </summary>
        public decimal GetValue() {
            decimal sum = 0;
            foreach (decimal count in _counters.Values)
                sum += count;
            return sum;
        }

        #endregion

        #region Implementation of ICRDT<GCounter<TKey>>

        /// <inheritdoc />
        public void Merge(GCounterDecimal<TKey> other) {
            foreach (var kvp in other._counters) {
                if (_counters.TryGetValue(kvp.Key, out decimal thisCount)) {
                    _counters[kvp.Key] = Math.Max(thisCount, kvp.Value); // resolve conflict: greater count wins
                } else {
                    _counters.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// <inheritdoc />
        public GCounterDecimal<TKey> Copy() {
            return new GCounterDecimal<TKey>(this);
        }

        #endregion

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(GCounterDecimal<TKey>)}, Value=" + GetValue();
        }

        #endregion
    }
}
