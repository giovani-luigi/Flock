using System;
using Flock.Core.Identification;
using MessagePack;

namespace Flock.Core.Network {

    [MessagePackObject()]
    public class Node : IUnique {
        
        [Key(0)]
        public UniqueId NodeId { get; }

        public Node(UniqueId nodeId) {
            if (nodeId.Equals(UniqueIdFactory.Default))
                throw new ArgumentException("The node can't use the default id");
            NodeId = nodeId;
        }

        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => NodeId;

        #endregion
    }
}
