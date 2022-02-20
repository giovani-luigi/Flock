using System;
using Flock.Core.Drivers.Cryptography;

namespace Flock.Core.Engines {
    internal class CryptographyEngine : IDisposable {
        
        private readonly ICryptographyDriver _driver;

        public CryptographyEngine(ICryptographyDriver driver) {
            _driver = driver;
        }
        
        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            _driver.Dispose();
        }

        #endregion
    }
}
