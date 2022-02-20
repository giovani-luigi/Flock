using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models;
using Flock.Core.Network;
using Flock.Core.Tests.Imp;
using Flock.Core.Tests.Utils;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Network {

    [TestFixture]
    public class ClientCounterTests {

        [Test]
        public void Test_Client_Counter_Seeds_Network_On_Change() {

            var config = new FlockConfigurationDefault();
            var middleware = new FlockMiddleware(config);
            var localNode = new Node(UniqueIdFactory.CreateNew());

            using (var client = middleware.CreateClient(localNode, Group.World)) {

                var counterId = UniqueIdFactory.CreateNew();
                var counter = client.GetCounter(counterId);

                counter.Increment();

                var packet = (config.NetworkDriver as NetworkDriverSimple)?.PublishedPackets.FirstOrDefault();

                Assert.That(packet, Is.Not.Null);

                Assert.That(packet!.ModelId, Is.EqualTo(counter.ModelId));
            }
        }
        
        [Test]
        public void Test_Client_GetCounter_Returns_Counter() {

            var middleware = FlockMiddlewareFactory.CreateTestDefault();

            var localNode = new Node(UniqueIdFactory.CreateNew());

            using (var client = middleware.CreateClient(localNode, Group.World)) {

                var counterId = UniqueIdFactory.CreateNew();
                var counter = client.GetCounter(counterId);

                Assert.That(counter, Is.InstanceOf<Counter>());

                Assert.That(counter.ModelId, Is.EqualTo(counterId));
            }
        }

        [Test]
        public void Test_Client_Counter_Receives_Updates_From_Adjacent_Nodes() {

            var net = new TestNetwork();

            var c1 = net.AddNew();
            var c2 = net.AddNew();

            var counterId = UniqueIdFactory.CreateNew();

            using (c1)
            using (c2) {
                var cnt1 = c1.GetCounter(counterId);
                var cnt2 = c2.GetCounter(counterId);

                cnt1.Increment();

                // check if propagated
                Assert.That(cnt2.GetValue(), Is.EqualTo(1));
            }
        }

        [Test]
        public void Test_Client_Counter_Query_Adjacent_For_Updates() {

            /*
              Create a counter with 'counterId' in client c2
              Increment counter on client c2
              Create client c1 and add to the network.
              Request counter 'counterId' from c1 to see if c1 query the network for updates that it may have missed
            */

            var net = new TestNetwork();
            var counterId = UniqueIdFactory.CreateNew();

            // client c2: counter has value = 1
            var c2 = net.AddNew();
            var cnt_c2 = c2.GetCounter(counterId);
            cnt_c2.Increment();
            
            // client c1: counter has value = 0
            var c1 = net.AddNew(); 

            using (c1)
            using (c2) {

                // trigger a query on the network
                // client c1 should end up receiving an update from adjacent node client c2
                var cnt_c1 = c1.GetCounter(counterId); 

                // client c1: should now have the value = 1 as in client c2
                Assert.That(cnt_c1.GetValue(), Is.EqualTo(1));

            }
        }
    }
}