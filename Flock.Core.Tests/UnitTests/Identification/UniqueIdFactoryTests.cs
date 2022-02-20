using Flock.Core.Identification;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Identification {
    
    [TestFixture]
    public class UniqueIdFactoryTests {


        [Test]
        public void Test_CreateNew_Create_Distinct() {

            var id1 = UniqueIdFactory.CreateNew();
            var id2 = UniqueIdFactory.CreateNew();

            Assert.That(id1, Is.Not.EqualTo(UniqueIdFactory.Default));
            Assert.That(id2, Is.Not.EqualTo(UniqueIdFactory.Default));

            Assert.That(id1, Is.Not.EqualTo(id2));

        }

        [Test]
        public void Test_Create_With_ParseBase64() {

            var id = UniqueIdFactory.CreateNew();
            
            Assert.That(UniqueIdFactory.ParseBase64(id.ToString()), Is.EqualTo(id));
        }

    }
}
