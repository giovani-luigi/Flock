using System;
using System.Threading.Tasks;
using Flock.Core.Drivers.Network;
using Flock.Core.Identification;
using Flock.Core.Network;

namespace Flock.Core.Synchronization {
    public abstract class ModelSynchronizer : IDisposable {
        
        protected readonly Client Client;

        protected readonly IUnique Model;

        protected ModelSynchronizer(Client client, IUnique model) {
            Client = client;
            Model = model;
        }

        protected abstract Hash GetCurrentStateSignatureImp(long part);

        public Hash GetCurrentStateSignature(long part) {
            return GetCurrentStateSignatureImp(part);
        }

        protected abstract Task SeedAsyncImp();

        public Task SeedAsync() {
            return SeedAsyncImp();
        }

        protected abstract Task QueryAsyncImp();

        public Task QueryAsync() {
            return QueryAsyncImp();
        }

        protected abstract Task MergeAsyncImp(NetworkPacket packet);

        public Task MergeAsync(NetworkPacket packet) {
            
            if (packet.ModelId != Model.UniqueId)
                throw new Exception("Model ID does not match the instance owned by this class");
            if (packet.Group != Client.Group)
                throw new Exception("Packet with incorrect group");

            return MergeAsyncImp(packet);
        }

        protected abstract Task PersistAsyncImp();

        public Task PersistAsync() {
            return PersistAsyncImp();
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
            }
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
