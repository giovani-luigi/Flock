using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Network;

namespace Flock.Core.Tests.Imp {
    internal class NetworkDriverSimple : INetworkDriver {

        private readonly List<NetworkPacket> _publishedPackets;
 
        public bool Disposed { get; private set; }

        public IReadOnlyList<NetworkPacket> PublishedPackets => new ReadOnlyCollection<NetworkPacket>(_publishedPackets);
        
        public NetworkDriverSimple() {
            _publishedPackets = new List<NetworkPacket>();
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
            _publishedPackets.Add(packet);
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
