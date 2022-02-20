using System.Collections.Generic;
using Flock.Core.Identification;
using Flock.Core.Network;
using Flock.Core.Tests.Imp;

namespace Flock.Core.Tests.Utils {

    /// <summary>
    /// Represents a network of nodes for testing purpose.
    /// This class offer methods to drive network drivers in order to test the middleware functionality
    /// </summary>
    public class TestNetwork {

        private readonly List<NetworkDriver> _drivers;

        public Group Group { get; }

        public TestNetwork() {
            _drivers = new List<NetworkDriver>();
            Group = Group.World;
        }

        public Client AddNew() {
            var node = new Node(UniqueIdFactory.CreateNew());
            var driver = new NetworkDriver(node);
            foreach (var registeredDriver in _drivers) {
                registeredDriver.AddAdjacentNode(driver);
                driver.AddAdjacentNode(registeredDriver);
            }
            _drivers.Add(driver);
            var mid = new FlockMiddleware(new FlockConfigurationDefault(driver));
            return mid.CreateClient(node, Group);
        }
        
    }
}
