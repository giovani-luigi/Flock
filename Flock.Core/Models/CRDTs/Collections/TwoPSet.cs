using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Collections {

    /// <summary>
    /// Two Phase Set (TwoPSet)
    /// </summary>
    /// <remarks>
    /// A Two Phase Set is a Conflict-free Replicated Data Type (CRDT) that allows insertion of unique items,
    /// and removal of item, but removed items cannot be re-added.
    /// The set contains no duplicate elements, and whose elements are in no particular order.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the data stored in the collection.
    /// </typeparam>
    [MessagePackObject()]
    public sealed class TwoPSet<T> : ICrdt<TwoPSet<T>> {
        
        #region State

        [Key(1)]
        private readonly HashSet<T> _added;

        [Key(2)]
        private readonly HashSet<T> _removed;

        #endregion

        #region Properties

        public IEnumerable<T> RemovedItems => _removed;

        public IEnumerable<T> AddedItems => _added;
        
        #endregion

        #region Constructors

        public TwoPSet() {
            _added = new HashSet<T>();
            _removed = new HashSet<T>();
        }

        public TwoPSet(TwoPSet<T> set) {
            _added = new HashSet<T>(set._added);
            _removed = new HashSet<T>(set._removed);
        }

        [SerializationConstructor]
        internal TwoPSet(HashSet<T> added, HashSet<T> removed) {
            _added = added;
            _removed = removed;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified item to the set
        /// </summary>
        /// <returns>
        /// true, if item has been added, false if the item is already present
        /// or has been removed once and therefore cannot be added back.
        /// </returns>
        public bool Add(T item) {
            if (_removed.Contains(item)) {
                return false;
            }
            return _added.Add(item);
        }

        /// <summary>
        /// Removes the specified item from the set if exists
        /// </summary>
        /// <returns>
        /// true, if the item has been removed, false if the item is not in the
        /// set of has been removed already.
        /// </returns>
        public bool Remove(T item) {
            if (_added.Remove(item)) {
                _removed.Add(item);
            }
            return false;
        }

        /// <summary>
        /// Get all items currently in the set
        /// </summary>
        public IReadOnlyCollection<T> GetItems() {
            return new ReadOnlyCollection<T>(_added.ToList());
        }

        public int Count() {
            return _added.Count;
        }

        #endregion

        #region Implementation of ICRDT<TwoPSet<T>>

        /// <inheritdoc />
        public void Merge(TwoPSet<T> other) {
            // first we merge all added items 
            _added.UnionWith(other._added);
            // merge all removed items
            _removed.UnionWith(other._removed);
            // clean-up removed items to reduce state size
            foreach (var item in _removed) {
                _added.Remove(item);
            }
        }

        /// <inheritdoc />
        public TwoPSet<T> Copy() {
            return new TwoPSet<T>(this);
        }

        #endregion

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(TwoPSet<T>)}, Added={_added}, Removed={_removed}";
        }

        #endregion
    }
}
