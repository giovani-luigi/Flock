using Flock.Core.Identification;
using Flock.Core.Models;
using Flock.Core.Network;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Synchronization {
    
    [TestFixture]
    public class CounterStateTests {

        [Test]
        public void Test_GetHash() {

            var counter = new Counter(UniqueIdFactory.CreateNew(), new Node(UniqueIdFactory.CreateNew()));

            counter.Increment(2500);
            counter.Decrement(1000);
            
            string hash = counter.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash = " + hash);

            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void Test_GetHash_Are_Same_For_Identical_Values_Different_ModelId() {

            var counter1 = new Counter(UniqueIdFactory.CreateNew(), new Node(UniqueIdFactory.CreateNew()));
            counter1.Increment(2500);
            counter1.Decrement(1000);

            var counter2 = new Counter(UniqueIdFactory.CreateNew(), new Node(UniqueIdFactory.CreateNew()));
            counter2.Increment(2500);
            counter2.Decrement(1000);

            string hash1 = counter1.GetState().GetStateSignature(0).Value;
            string hash2 = counter2.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1 + "\nHash2 = " + hash2);

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void Test_GetHash_Are_Dfiferent_For_Different_Values() {

            var counter1 = new Counter(UniqueIdFactory.CreateNew(), new Node(UniqueIdFactory.CreateNew()));
            counter1.Increment(2500);
            counter1.Decrement(1000);

            var counter2 = new Counter(UniqueIdFactory.CreateNew(), new Node(UniqueIdFactory.CreateNew()));
            counter2.Increment(1000);
            counter2.Decrement(2500);

            string hash1 = counter1.GetState().GetStateSignature(0).Value;
            string hash2 = counter2.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1 + "\nHash2 = " + hash2);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

    }
}
