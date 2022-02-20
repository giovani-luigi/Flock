using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Network;

namespace Flock.Core.Engines {
    internal class NetworkEngine : IDisposable {
      
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        private readonly IEnumerable<INetworkDriver> _drivers;

        public NetworkEngine(IEnumerable<INetworkDriver> drivers) {
            _drivers = drivers ?? throw new ArgumentNullException(nameof(drivers));
            foreach (var networkDriver in drivers) {
                networkDriver.PacketReceived += NetworkDriverOnPacketReceived;
            }
        }

        public Task PublishAsync(NetworkPacket packet, CancellationToken cancellationToken) {
            var tasks = new List<Task>();
            foreach (var networkDriver in _drivers) {
                tasks.Add(networkDriver.PublishAsync(packet, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }

        private void NetworkDriverOnPacketReceived(object sender, PacketReceivedEventArgs e) {
            PacketReceived?.Invoke(sender, e); // forward and preserve sender
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            foreach (var networkDriver in _drivers) {
                networkDriver.Dispose();
            }
        }

        #endregion

    }
}
