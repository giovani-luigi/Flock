using Flock.Core.Tests.Imp;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests {
    internal class FlockMiddlewareTests {
        
        [Test]
        public void Test_Drivers_Are_Disposed_When_Middleware_Is_Disposed() {

            var config = new FlockConfigurationDefault();

            using (new FlockMiddleware(config)) { }
            
            Assert.That(config.StorageDriver, Is.AssignableTo<StorageDriverDefault>());

            Assert.That(((StorageDriverDefault)config.StorageDriver).Disposed, Is.True);

            foreach (var networkDriver in config.NetworkDrivers) {
                Assert.That(networkDriver, Is.AssignableTo<NetworkDriverSimple>());
                Assert.That(((NetworkDriverSimple) networkDriver).Disposed, Is.True);
            }
        }

    }
}