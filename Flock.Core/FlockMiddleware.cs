using System;
using Flock.Core.Engines;
using Flock.Core.Network;
using Flock.Core.Utils;

namespace Flock.Core {
    
    /// <summary>
    /// Represents the Composition Root of the middleware
    /// </summary>
    public class FlockMiddleware : IDisposable {
        
        private readonly FlockConfiguration _configuration;

        internal NetworkEngine Network { get; }
        internal StorageEngine Storage { get; }
        internal CryptographyEngine  Cryptography { get; }
        internal ILog Log => _configuration.Logger;

        public FlockMiddleware(FlockConfiguration configuration) {
            _configuration = configuration;
            Network = new NetworkEngine(_configuration.NetworkDrivers);
            Storage = new StorageEngine(_configuration.StorageDriver);
            Cryptography = new CryptographyEngine(_configuration.CryptographyDriver);
        }

        public Client CreateClient(Node node, Group group) {
            return new Client(this, node, group);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            Network?.Dispose();
            Storage?.Dispose();
            Cryptography?.Dispose();
        }

        #endregion
    }
}
