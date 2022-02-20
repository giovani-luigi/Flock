using System;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Counters {

    /// <summary>
    /// Positive Negative Counter (PN-Counter)
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
    public sealed class PNCounter<TKey> : ICrdt<PNCounter<TKey>> {

        #region State
        
        [Key(0)]
        private readonly GCounter<TKey> _inc;
        
        [Key(1)]
        private readonly GCounter<TKey> _dec;

        #endregion

        #region Properties

        [IgnoreMember]
        public ulong IncrementValue => _inc.GetValue();

        [IgnoreMember]
        public ulong DecrementValue => _dec.GetValue();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="PNCounter{TKey}"/> with initial count set to Zero.
        /// </summary>
        public PNCounter() {
            _inc = new GCounter<TKey>();
            _dec = new GCounter<TKey>();
        }
        
        /// <summary>
        /// Creates a <see cref="PNCounter{TKey}"/> with data copied from the specified <paramref name="counter"/>
        /// </summary>
        public PNCounter(PNCounter<TKey> counter) {
            _inc = new GCounter<TKey>(counter._inc);
            _dec = new GCounter<TKey>(counter._dec);
        }

        internal PNCounter(GCounter<TKey> inc, GCounter<TKey> dec) {
            _inc = inc;
            _dec = dec;
        }

        #endregion

        #region Public Methods

        public void Increment(TKey key, ulong increment = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (increment == 0) return; // handle politely
            _inc.Increment(key, increment);
        }

        public void Decrement(TKey key, ulong decrement = 1) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (decrement == 0) return; // handle politely
            _dec.Increment(key, decrement);
        }

        /// <summary>
        /// Gets the total 
        /// </summary>
        public long GetValue() {
            return (long) (_inc.GetValue() - _dec.GetValue());
        }

        #endregion

        #region Implementation of ICRDT<PNCounter<TKey>>

        /// <inheritdoc />
        public void Merge(PNCounter<TKey> other) {
            _inc.Merge(other._inc);
            _dec.Merge(other._dec);
        }

        /// <inheritdoc />
        public PNCounter<TKey> Copy() {
            return new PNCounter<TKey>(this);
        }

        #endregion
        
        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(PNCounter<TKey>)}, Value=" + GetValue();
        }

        #endregion
    }
}
