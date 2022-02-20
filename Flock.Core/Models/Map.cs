using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Collections;
using Flock.Core.Models.CRDTs.Registers;
using Flock.Core.Synchronization;

namespace Flock.Core.Models 
{
    /// <summary>
    /// An unsorted collection of unique items
    /// </summary>
    public class Map<T> : IModel<Map<T>> where T : IUnique {

        private readonly object _monitor;

        private readonly TwoPSet<UniqueId> _keys;
        private readonly Dictionary<UniqueId, BWRegister<T>> _data;

        public UniqueId ModelId { get; }

        #region Constructors

        private Map() {
            _monitor = new object();
            _keys = new TwoPSet<UniqueId>();
            _data = new Dictionary<UniqueId, BWRegister<T>>();
        }

        public Map(UniqueId id) : this() {
            ModelId = id;
        }

        #endregion

        private BWRegister<T> WrapDataIntoRegister(T data) {
            var register = new BWRegister<T> {
                VersionGenerator = new DateTimeVersionGenerator()
            };
            register.Set(data);
            return register;
        }

        #region Public Methods

        /// <summary>
        /// Sets a value (insert/update)
        /// </summary>
        /// <returns>true if value was set, false if was not possible to set, because the item has been removed</returns>
        public bool Set(T data) {
            bool updated = false;
            lock (_monitor) {
                if (_keys.Add(data.UniqueId)) {
                    _data.Add(data.UniqueId, WrapDataIntoRegister(data));
                    updated = true;
                } else {
                    if (_data.TryGetValue(data.UniqueId, out var register)) {
                        register.Set(data);
                        updated = true;
                    }
                }
            }
            if (updated) RaiseEventStateChanged();
            return updated;
        }

        public bool Remove(UniqueId key)
        {
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
                return new ReadOnlyCollection<T>(_data.Select(x => x.Value.Data).ToList());
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
                return new MapState<T>(_keys, _data);
            }
        }

        /// <inheritdoc />
        public void Merge(IModelState state)
        {
            if (!(state is MapState<T> feedState)) return;

            // validate provided state, i.e. ensure all its keys have respective data
            feedState.ThrowIfCorruptedState();

            lock (_monitor) {

                // merge keys
                _keys.Merge(feedState.Keys);

                // in data collection, merge, selecting distinct keys, with biggest version
                foreach (var key in _keys.GetItems())
                {
                    // if this instance does have data with the current key
                    if (_data.TryGetValue(key, out var thisData)) {
                        // if other instance also has data with the current key, keep biggest version
                        if (feedState.Data.TryGetValue(key, out var otherData)) {
                            _data.Add(key, thisData.Version > otherData.Version ? thisData : otherData);
                        } else {
                            // keep data from the state of this instance (nothing should be added, since its already in the collection)
                        }
                    } else { // if this instance's state does not have data with the current key
                        // provided state instance has the data for the key, use it
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

        private void RaiseEventStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
