using System;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Storage;
using Flock.Core.Identification;

namespace Flock.Core.Engines {
    internal class StorageEngine : IDisposable {
        
        private readonly IStorageDriver _driver;

        public StorageEngine(IStorageDriver driver) {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public Task StoreAsync(UniqueId modelId, UniqueId stateId, byte[] payload, CancellationToken cancellation) {
            if (modelId == null) throw new ArgumentNullException(nameof(modelId));
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            
            // if state ID is not give, just use default key
            if (stateId == null) stateId = UniqueIdFactory.Default;

            var kv = _driver.KeyValues;
            if (kv == null) throw new Exception("Storage driver did not provide required feature.");

            return kv.StoreAsync(new DualKeyValue(modelId, stateId, payload), cancellation);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            _driver?.Dispose();
        }

        #endregion
    }
}
