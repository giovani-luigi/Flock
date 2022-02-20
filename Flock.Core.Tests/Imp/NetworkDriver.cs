using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Network;
using Flock.Core.Network;

namespace Flock.Core.Tests.Imp {
    internal class NetworkDriver : INetworkDriver {

        private readonly List<NetworkDriver> _adjacent;

        public bool Disposed { get; private set; }

        public Node Node { get; }

        public NetworkDriver(Node ownerNode) {
            _adjacent = new List<NetworkDriver>();
            Node = ownerNode;
        }

        public void AddAdjacentNode(NetworkDriver driver) {
            _adjacent.Add(driver);
        }
        
        internal void RaisePacketReceived(PacketReceivedEventArgs e) {
            PacketReceived?.Invoke(this, e);
        }

        #region Implementation of INetworkDriver

        /// <inheritdoc />
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        /// <inheritdoc />
        public bool Enabled { get; } = true;
        
        public Task PublishAsync(NetworkPacket packet, CancellationToken cancellationToken) {
            foreach (var driver in _adjacent) {
                if (driver.Node != packet.Author) {
                    driver.RaisePacketReceived(new PacketReceivedEventArgs(packet));
                }
            }
            return Task.CompletedTask;
        }

        #endregion
        
        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            Disposed = true;
        }

        #endregion

    }
}
