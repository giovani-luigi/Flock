using Flock.Core.Identification;
using Flock.Core.Network;

namespace Flock.Core.Drivers.Network {
    public static class NetworkPacketFactory {

        private static UniqueId GetNextPacketId() {
            return UniqueIdFactory.CreateNew();
        }

        #region Static

        public static NetworkPacket CreateCounterSeed(Node source, Group group, UniqueId modelId, Hash stateSignature, byte[] payload) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.CounterSeed, modelId, stateSignature, 0, payload);
        }

        public static NetworkPacket CreateCounterQuery(Node source, Group group, UniqueId modelId) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.CounterQuery, modelId, Hash.Empty, 0, null);
        }

        public static NetworkPacket CreateFeedSeed(Node source, Group group, UniqueId modelId, Hash stateSignature, byte[] payload) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.FeedSeed, modelId, stateSignature, 0, payload);
        }

        public static NetworkPacket CreateFeedQuery(Node source, Group group, UniqueId modelId) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.FeedQuery, modelId, Hash.Empty, 0, null);
        }

        public static NetworkPacket CreateMapSeed(Node source, Group group, UniqueId modelId, Hash stateSignature, byte[] payload) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.MapSeed, modelId, stateSignature, 0, payload);
        }

        public static NetworkPacket CreateMapQuery(Node source, Group group, UniqueId modelId) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.MapQuery, modelId, Hash.Empty, 0, null);
        }

        public static NetworkPacket CreateBlobSeed(Node source, Group group, UniqueId modelId, Hash stateSignature, byte[] payload) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.BlobSeed, modelId, stateSignature, 0, payload);
        }

        public static NetworkPacket CreateBlobQuery(Node source, Group group, UniqueId modelId) {
            return new NetworkPacket(GetNextPacketId(), source, group, PacketType.BlobQuery, modelId, Hash.Empty, 0, null);
        }

        #endregion

    }
}
