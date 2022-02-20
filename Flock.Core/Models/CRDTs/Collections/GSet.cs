using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Collections {

    /// <summary>
    /// Grow only set (G-Set)
    /// </summary>
    /// <remarks>
    /// A G-Set is a Conflict-free Replicated Data Type (CRDT) that allows insertion of unique items.
    /// These sets can only grow and do not support removal of elements.
    /// A G-Set is a collection that contains no duplicate elements, and whose elements are in no particular order.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the data stored in the collection.
    /// </typeparam>
    [MessagePackObject()]
    public sealed class GSet<T> : ICrdt<GSet<T>> {

        #region State

        [Key(1)]
        private readonly HashSet<T> _items;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an empty <see cref="GSet{T}"/>.
        /// </summary>
        public GSet() {
            _items = new HashSet<T>();
        }

        /// <summary>
        /// Creates a <see cref="GSet{T}"/> with data copied from the specified <paramref name="set"/>
        /// </summary>
        public GSet(GSet<T> set) {
            _items = new HashSet<T>(set._items);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified item to the set
        /// </summary>
        /// <returns>
        /// true, if element has been added, false if the element is already present in the set
        /// </returns>
        public bool Add(T item) {
            return _items.Add(item);
        }

        /// <summary>
        /// Determines whether this set contains a specified item
        /// </summary>
        public bool Contains(T item) {
            return _items.Contains(item);
        }

        /// <summary>
        /// Gets all the items currently in the set as a read-only collection
        /// </summary>
        public IReadOnlyCollection<T> GetItems() {
            return new ReadOnlyCollection<T>(_items.ToList());
        }

        /// <summary>
        /// Gets the count of items in the set
        /// </summary>
        public int Count() {
            return _items.Count;
        }

        #endregion
        
        #region Implementation of ICRDT<GSet<TKey>>

        /// <inheritdoc />
        public void Merge(GSet<T> other) {
            _items.UnionWith(other._items);
        }

        /// <inheritdoc />
        public GSet<T> Copy() {
            return new GSet<T>(this);
        }

        #endregion

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString() {
            return $"{nameof(GSet<T>)}, Items Count={Count()}";
        }

        #endregion
        
    }
}
