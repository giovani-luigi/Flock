using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Collections;
using Flock.Core.Models.CRDTs.Registers;
using Flock.Core.Utils;
using MessagePack;

namespace Flock.Core.Synchronization
{
    public class MapState<T> : IModelState where T : IUnique {

        #region State

        [Key(0)]
        public readonly TwoPSet<UniqueId> Keys;

        [Key(1)]
        public readonly ReadOnlyDictionary<UniqueId, BWRegister<T>> Data;

        #endregion

        #region Constructors

        [SerializationConstructor]
        public MapState(TwoPSet<UniqueId> keys, IDictionary<UniqueId, BWRegister<T>> dictionary) {
            Keys = keys.Copy();
            Data = new ReadOnlyDictionary<UniqueId, BWRegister<T>>(dictionary);
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
        public Hash GetStateSignature(long part)
        {
            // calculate the checksum of the set by
            // iterating through all members of the set
            var state1 = UniqueIdFactory.Combine(Keys.AddedItems).ToString();
            var state2 = UniqueIdFactory.Combine(Keys.RemovedItems).ToString();
            // add the count of the set to minimize collision when identical checksum
            var state3 = Keys.AddedItems.Count().ToString("X");
            var state4 = Keys.RemovedItems.Count().ToString("X");

            // append to the hash all items versions
            byte[] dataVersionsHashBytes = new byte[UniqueId.LENGTH];
            foreach (var key in Keys.GetItems()) {
                
                // the hash of each map entry, uses its unique ID and its version
                var data = Data[key];
                var ver = data.Version;
                var bytes = key.GetBytes().ToArray();  // 128-bit
                var verBytes = BitConverter.GetBytes(ver); // 64-bit

                // we need to mix the key with the version in order to increase entropy
                // otherwise version is only 8-bit long which is not good to prevent collisions
                // this way all key bytes will be influenced by the version
                for (int i = 0; i < 16; i += 8) {
                    for (int j = 0; j < 8; j++) {
                        bytes[i + j] = (byte)(bytes[i + j] ^ verBytes[j]);
                    }
                }

                // combine the array of the current item with previous items, using a XOR function
                dataVersionsHashBytes = ByteArray.Xor(dataVersionsHashBytes, bytes.ToArray());
            }

            // join strings into a single hash
            return new Hash(string.Join(";", state1, state2, state3, state4, Convert.ToBase64String(dataVersionsHashBytes)));
        }


        #endregion

    }
}
