using System.Collections.Generic;
using Flock.Core.Drivers.Cryptography;
using Flock.Core.Drivers.Network;
using Flock.Core.Drivers.Storage;
using Flock.Core.Identification;
using Flock.Core.Utils;

namespace Flock.Core {
    
    public abstract class FlockConfiguration {
        
        public abstract UniqueId NetworkId { get; }

        public abstract ILog Logger { get; }

        public abstract IStorageDriver StorageDriver { get; }

        public abstract IEnumerable<INetworkDriver> NetworkDrivers { get; }
        
        public abstract ICryptographyDriver CryptographyDriver { get; }

    }
}
