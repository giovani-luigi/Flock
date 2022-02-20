using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Collections;
using MessagePack;

namespace Flock.Core.Synchronization {

    public class FeedState<T> : IModelState where T : IUnique {

        #region State

        [Key(0)]
        public readonly TwoPSet<UniqueId> Keys;

        [Key(1)]
        public readonly ReadOnlyDictionary<UniqueId, T> Data;
        
        #endregion

        #region Constructors

        [SerializationConstructor]
        public FeedState(TwoPSet<UniqueId> keys, IDictionary<UniqueId, T> dictionary) {
            Keys = keys.Copy();
            Data = new ReadOnlyDictionary<UniqueId, T>(dictionary);
        }

        #endregion

        /// <summary>
        /// Validate provided state, i.e. ensure all its keys have respective data.
        /// If not valid, throw.
        /// </summary>
        /// <exception cref="Exception">When state is not valid</exception>
        public void ThrowIfCorruptedState() {
            if (Keys.GetItems().Any(feedStateKey => !Data.ContainsKey(feedStateKey))) {
                throw new Exception("Corrupted state");
            }
        }

        #region Implementation of IModelState

        /// <inheritdoc />
        public bool IsEmpty => Keys.Count() == 0;

        /// <inheritdoc />
        public Hash GetStateSignature(long part) {
            // calculate the checksum of the set by
            // iterating through all members of the set
            var state1 = UniqueIdFactory.Combine(Keys.AddedItems).ToString();
            var state2 = UniqueIdFactory.Combine(Keys.RemovedItems).ToString();
            // add the count of the set to minimize collision when identical checksum
            var state3 = Keys.AddedItems.Count().ToString("X");
            var state4 = Keys.RemovedItems.Count().ToString("X");
            // join strings into a single hash
            return new Hash(string.Join(";", state1, state2, state3, state4));
        }

        #endregion
        
    }
}
