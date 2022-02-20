using System.Collections.Generic;
using Flock.Core.Drivers.Cryptography;
using Flock.Core.Drivers.Network;
using Flock.Core.Drivers.Storage;
using Flock.Core.Identification;
using Flock.Core.Tests.Utils;
using Flock.Core.Utils;

namespace Flock.Core.Tests.Imp {
    internal class FlockConfigurationDefault : FlockConfiguration {
        
        public INetworkDriver NetworkDriver { get; } 

        public FlockConfigurationDefault() {
            NetworkDriver = new NetworkDriverSimple();
            NetworkDrivers = new List<INetworkDriver> {
                NetworkDriver
            };
        }

        public FlockConfigurationDefault(INetworkDriver networkDriver) {
            NetworkDriver = networkDriver;
            NetworkDrivers = new List<INetworkDriver> {
                NetworkDriver
            };
        }

        #region Overrides of FlockConfiguration

        /// <inheritdoc />
        public override UniqueId NetworkId { get; } = UniqueIdFactory.CreateNew();

        /// <inheritdoc />
        public override IEnumerable<INetworkDriver> NetworkDrivers { get; }

        /// <inheritdoc />
        public override ILog Logger { get; } = new TestLogger();

        /// <inheritdoc />
        public override IStorageDriver StorageDriver { get; } = new StorageDriverDefault();
        
        /// <inheritdoc />
        public override ICryptographyDriver CryptographyDriver { get; } = new NoCryptographyDriver();

        #endregion

    }
}
