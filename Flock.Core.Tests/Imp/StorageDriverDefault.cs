using Flock.Core.Drivers.Storage;

namespace Flock.Core.Tests.Imp {
    internal class StorageDriverDefault : IStorageDriver {

        public bool Disposed { get; private set; }

        public StorageDriverDefault() {
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            Disposed = true;
        }

        #endregion

        #region Implementation of IStorageDriver

        /// <inheritdoc />
        public IRepositoryKeyValue KeyValues { get; } = new RepositoryKeyValue();

        #endregion
    }
}
