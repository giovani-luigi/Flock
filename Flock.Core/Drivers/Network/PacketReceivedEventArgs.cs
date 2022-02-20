using System;

namespace Flock.Core.Drivers.Network {
    public class PacketReceivedEventArgs : EventArgs {
        
        public NetworkPacket Packet { get; }

        public PacketReceivedEventArgs(NetworkPacket packet) {
            Packet = packet;
        }

    }
}
