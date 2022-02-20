using System;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Counters {

    /// <summary>
    /// Positive Negative Counter supporting decimal values. (PN-CounterDecimal)
    /// (by Giovani Luigi)
    /// </summary>
    /// <remarks>
    /// A PN-Counter is a Conflict-free Replicated Data Type (CRDT) that allows integer positive increments and decrements
    /// PN-Counters allow the counter to be decreased by tracking the increments (P) separate from the decrements (N), both represented as internal G-Counters.
    /// Merge is handled by merging the internal P and N counters.
    /// The value of the counter is the value of the P counter minus the value of the N counter.
    /// </remarks>
    /// <typeparam name="TKey">
    /// The key used to associate with a particular counter.
    /// </typeparam>
    [MessagePackObject()]
    public sealed class PNCounterDecimal<TKey> : ICrdt<PNCounterDecimal<TKey>> {

        #region State

        [Key(0)]
        private readonly GCounterDecimal<TKey> _inc;

        [Key(1)]
        private readonly GCounterDecimal<TKey> _dec;

        #endregion

        #region Properties

        [IgnoreMember]
        public decimal IncrementValue => _inc.GetValue();

        [IgnoreMember]
        public decimal DecrementValue => _dec.GetValue();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="PNCounterDecimal{TKey}"/> with initial count set to Zero.
        /// </summary>
        public PNCounterDecimal() {
            _inc = new GCounterDecimal<TKey>();
            _dec = new GCounterDecimal<TKey>();
        }

        /// <summary>
        /// Creates a <see cref="PNCounterDecimal{TKey}"/> with data copied from the specified <paramref name="counter"/>
        /// </summary>
        public PNCounterDecimal(PNCounterDecimal<TKey> counter) {
            _inc = new GCounterDecimal<TKey>(counter._inc);
            _dec = new GCounterDecimal<TKey>(counter._dec);
        }

        [SerializationConstructor]
        internal PNCounterDecimal(GCounterDecimal<TKey> inc, GCounterDecimal<TKey> dec) {
            _inc = inc;
            _dec = dec;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increment the counter
        /// </summary>
        /// <param name="increment">A positive number to increment</param>
        /// <exception cref="ArgumentNullException">When key is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">When increment is smaller than zero.</exception>
        public void Increment(TKey key, decimal increment = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (increment < 0) throw new ArgumentOutOfRangeException(nameof(increment), "Increment must be positive.");
            if (increment == 0) return; // handle politely
            _inc.Increment(key, increment);
        }


        /// <summary>
        /// Decrement the counter
        /// </summary>
        /// <param name="decrement">A positive number to decrement</param>
        /// <exception cref="ArgumentNullException">When key is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">When decrement is smaller than zero.</exception>
        public void Decrement(TKey key, decimal decrement = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (decrement < 0) throw new ArgumentOutOfRangeException(nameof(decrement), "Decrement must be positive.");
            if (decrement == 0) return; // handle politely
            _dec.Increment(key, decrement);
        }

        /// <summary>
        /// Gets the total 
        /// </summary>
        public decimal GetValue() {
            return _inc.GetValue() - _dec.GetValue();
        }

        #endregion

        #region Implementation of ICRDT<PNCounter<TKey>>

        /// <inheritdoc />
        public void Merge(PNCounterDecimal<TKey> other) {
            _inc.Merge(other._inc);
            _dec.Merge(other._dec);
        }

        /// <inheritdoc />
        public PNCounterDecimal<TKey> Copy() {
            return new PNCounterDecimal<TKey>(this);
        }

        #endregion

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(PNCounterDecimal<TKey>)}, Value=" + GetValue();
        }

        #endregion
    }
}
