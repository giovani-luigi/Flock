using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flock.Core.Drivers.Network {
    public interface INetworkDriver : IDisposable {

        event EventHandler<PacketReceivedEventArgs> PacketReceived;

        bool Enabled { get; }

        Task PublishAsync(NetworkPacket packet, CancellationToken cancellationToken);

    }
}
