using System;
using Flock.Core.Identification;
using Flock.Core.Network;
using MessagePack;

namespace Flock.Core.Drivers.Network {
    
    [MessagePackObject()]
    public class NetworkPacket : IUnique {
        
        /// <summary>
        /// The packet ID identifies this network packet uniquelly in the network.
        /// It is mainly used to track the packet route and for handshake signals.
        /// </summary>
        [Key(0)]
        public UniqueId PacketId { get; }

        /// <summary>
        /// The device that created the packet.
        /// </summary>
        [Key(1)]
        public Node Author { get; }

        /// <summary>
        /// The network group where this packet belongs in.
        /// The network group must match for a packet to be received by a node.
        /// </summary>
        [Key(2)]
        public Group Group { get; }

        /// <summary>
        /// The type of the packet.
        /// </summary>
        [Key(3)]
        public PacketType PacketType { get; }
        
        /// <summary>
        /// The unique identifier of the data model associated with this packet.
        /// </summary>
        [Key(4)]
        public UniqueId ModelId { get; }

        /// <summary>
        /// The state signature. This value is dependent on the data model.
        /// The synchronization objects use this field mainly to identify if a state is identical to other without processing model data.
        /// </summary>
        [Key(5)]
        public Hash StateSignature { get; }

        /// <summary>
        /// The sequential number.
        /// This number is used when the model state is composed by multiple fragments.
        /// </summary>
        [Key(6)]
        public long Part { get; }

        /// <summary>
        /// The serialized model state
        /// </summary>
        [Key(7)]
        public byte[] Payload { get; }

        [SerializationConstructor]
        internal NetworkPacket(UniqueId packetId, Node author, Group group, PacketType packetType, UniqueId modelId, Hash stateSignature, int part, byte[] payload) {
            PacketId = packetId ?? throw new ArgumentNullException(nameof(packetId));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Group = group;
            PacketType = packetType;
            ModelId = modelId ?? throw new ArgumentNullException(nameof(modelId));
            StateSignature = stateSignature;
            Part = part;
            Payload = payload;
        }
        
        #region Implementation of IUnique
        UniqueId IUnique.UniqueId => PacketId;

        #endregion
    }
}
