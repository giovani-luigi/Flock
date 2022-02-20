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
    internal class MapSynchronizer<T> : ModelSynchronizer where T : IUnique
    {
        private readonly Map<T> _map;
        private readonly ISerializer<MapState<T>> _serializer;
        private readonly CancellationTokenSource _cancellation;
        private readonly object _stateLock = new object();

        // cache serialized bytes and only update when required, i.e. when signature is different
        private byte[] _stateBytes;
        private Hash _stateSignature;

        public MapSynchronizer(Client client, Map<T> map) : base(client, map) {
            _map = map;
            _cancellation = new CancellationTokenSource();
            _serializer = new MapStateSerializer<T>();
            var initialState = (MapState<T>)_map.GetState();
            _stateBytes = _serializer.Serialize(initialState);
            _stateSignature = initialState.GetStateSignature(0);
        }

        private byte[] GetStateSerialized() {
            lock (_stateLock) {
                var state = (MapState<T>)_map.GetState();
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
            var packet = NetworkPacketFactory.CreateMapSeed(Client.Node, Client.Group, _map.ModelId, _stateSignature, state);
            return Client.Middleware.Network.PublishAsync(packet, _cancellation.Token);
        }

        protected override Task QueryAsyncImp() {
            // query the network
            return Client.Middleware.Network.PublishAsync(
                NetworkPacketFactory.CreateMapQuery(Client.Node, Client.Group, _map.ModelId),
                _cancellation.Token
                );
        }

        protected override Task MergeAsyncImp(NetworkPacket packet) {
            if (packet.PacketType != PacketType.MapQuery && packet.PacketType != PacketType.MapSeed)
                throw new Exception("Packet type cannot be handled by this class");
            var state = _serializer.Deserialize(packet.Payload);
            _map.Merge(state);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task PersistAsyncImp() {
            var state = GetStateSerialized();
            if (state is null) return Task.CompletedTask;
            if (state.Length == 0) return Task.CompletedTask;
            return Client.Middleware.Storage.StoreAsync(_map.ModelId, null, state, _cancellation.Token);
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
