using System;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Network;
using Flock.Core.Identification;
using Flock.Core.Models;
using Flock.Core.Network;
using Flock.Core.Serialization;

namespace Flock.Core.Synchronization
{
    internal class BlobSynchronizer : ModelSynchronizer {

        private readonly Blob _blob;
        private readonly ISerializer<BlobState> _serializer;
        private readonly CancellationTokenSource _cancellation;
        private readonly object _stateLock = new object();

        // cache serialized bytes and only update when required, i.e. when signature is different
        private byte[] _stateBytes;
        private Hash _stateSignature;

        public BlobSynchronizer(Client client, Blob blob) : base(client, blob) {
            _blob = blob;
            _cancellation = new CancellationTokenSource();
            _serializer = new BlobStateSerializer();
            var initialState = (BlobState)_blob.GetState();
            _stateBytes = _serializer.Serialize(initialState);
            _stateSignature = initialState.GetStateSignature(0);
        }

        private byte[] GetStateSerialized() {
            lock (_stateLock) {
                var state = (BlobState)_blob.GetState();
                var currSignature = state.GetStateSignature(0);
                if (currSignature != _stateSignature) {
                    _stateBytes = _serializer.Serialize(state);
                    _stateSignature = currSignature;
                }
            }
            return _stateBytes;
        }

        /// <inheritdoc />
        protected override Hash GetCurrentStateSignatureImp(long part) {
            return _stateSignature;
        }

        protected override Task SeedAsyncImp() {
            var state = GetStateSerialized();
            if (state.Length == 0) return Task.CompletedTask;
            var packet = NetworkPacketFactory.CreateBlobSeed(Client.Node, Client.Group, _blob.ModelId, _stateSignature, state);
            return Client.Middleware.Network.PublishAsync(packet, _cancellation.Token);
        }

        protected override Task QueryAsyncImp() {
            // query the network
            return Client.Middleware.Network.PublishAsync(
                NetworkPacketFactory.CreateBlobQuery(Client.Node, Client.Group, _blob.ModelId),
                _cancellation.Token
                );
        }

        protected override Task MergeAsyncImp(NetworkPacket packet) {
            if (packet.PacketType != PacketType.BlobQuery && packet.PacketType != PacketType.BlobSeed)
                throw new Exception("Packet type cannot be handled by this class");
            var state = _serializer.Deserialize(packet.Payload);
            _blob.Merge(state);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task PersistAsyncImp() {
            var state = GetStateSerialized();
            if (state is null) return Task.CompletedTask;
            if (state.Length == 0) return Task.CompletedTask;
            return Client.Middleware.Storage.StoreAsync(_blob.ModelId, null, state, _cancellation.Token);
        }

        #region Overrides of ModelSynchronizer

        /// <inheritdoc />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _cancellation.Cancel();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
