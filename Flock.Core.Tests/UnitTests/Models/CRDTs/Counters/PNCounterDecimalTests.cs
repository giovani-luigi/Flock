using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Counters;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Models.CRDTs.Counters {
    
    [TestFixture]
    public class PNCounterDecimalTests {

        [Test]
        public void Test_Serialize_Deserialize() {

            var key = UniqueIdFactory.Default;
            var obj = new PNCounterDecimal<UniqueId>();

            obj.Increment(key);

            var serializer = new Serialization.MessagePackSerializer<PNCounterDecimal<UniqueId>>();
            var serialized = serializer.Deserialize(serializer.Serialize(obj));

            Assert.That(serialized.GetValue(), Is.EqualTo(1));
            
        }
    }
}
