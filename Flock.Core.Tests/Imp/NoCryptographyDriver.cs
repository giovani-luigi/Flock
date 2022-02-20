using Flock.Core.Drivers.Cryptography;

namespace Flock.Core.Tests.Imp {
    public class NoCryptographyDriver : ICryptographyDriver {

        public NoCryptographyDriver() {
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() { }

        #endregion
    }
}
