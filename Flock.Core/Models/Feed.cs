using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Collections;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {

    /// <summary>
    /// An unsorted collection of unique items
    /// </summary>
    public class Feed<T> : IModel<Feed<T>> where T : IUnique {
        
        private readonly object _monitor;
        
        private readonly TwoPSet<UniqueId> _keys;
        private readonly Dictionary<UniqueId, T> _data;

        public UniqueId ModelId { get; }

        #region Constructors

        private Feed() {
            _monitor = new object();
            _keys = new TwoPSet<UniqueId>();
            _data = new Dictionary<UniqueId, T>();
        }

        public Feed(UniqueId id) : this() {
            ModelId = id;
        }
        
        #endregion
        
        #region Public Methods

        public void Append(T data) {
            lock (_monitor) {
                if (_keys.Add(data.UniqueId)) {
                    _data.Add(data.UniqueId, data);
                }
            }
            RaiseEventStateChanged();
        }

        public bool Remove(UniqueId key) {
            bool removed = false;
            lock (_monitor) {
                if (_keys.Remove(key)) {
                    _data.Remove(key);
                    removed = true;
                }
            }
            if (removed) RaiseEventStateChanged();
            return removed;
        }
        
        public IReadOnlyList<T> GetItems() {
            lock (_monitor) {
                return new ReadOnlyCollection<T>(_data.Values.ToList()); 
            }
        }

        #endregion
        
        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => ModelId;

        #endregion

        #region Implementation of IModel<Feed<T>>

        /// <inheritdoc />
        public event EventHandler StateChanged;

        /// <inheritdoc />
        public IModelState GetState() {
            lock (_monitor) {
                return new FeedState<T>(_keys, _data);
            }
        }

        /// <inheritdoc />
        public void Merge(IModelState state) {
            if (!(state is FeedState<T> feedState)) return;

            // validate provided state, i.e. ensure all its keys have respective data
            feedState.ThrowIfCorruptedState();
            
            lock (_monitor) {
                
                // merge keys
                _keys.Merge(feedState.Keys);
                
                // merge data collection
                foreach (var key in _keys.GetItems()) {
                    // if this instance's state does not have data with the current key, get from the provided state
                    if (!_data.TryGetValue(key, out _)) {
                        // if provided state instance has the data for the key, use it
                        if (feedState.Data.TryGetValue(key, out var otherData)) {
                            _data.Add(key, otherData);
                        } else {
                            // unexpected since provided state has been validated
                            throw new Exception("Corrupted model State");
                        }
                    } 
                }
                
                // remove deleted items (garbage collection)
                foreach (var removedItemKey in _keys.RemovedItems) {
                    _data.Remove(removedItemKey);
                }
            }

            RaiseEventStateChanged();
        }

        #endregion

        private void RaiseEventStateChanged() {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
