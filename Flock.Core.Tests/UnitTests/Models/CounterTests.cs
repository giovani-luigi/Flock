using System;
using Flock.Core.Identification;
using Flock.Core.Models;
using Flock.Core.Network;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Models {
    
    [TestFixture]
    public class CounterTests {

        private Counter CreateCounter() {
            return new Counter(
                modelId: UniqueIdFactory.CreateNew(),
                localNode: new Node(UniqueIdFactory.CreateNew()));
        }

        [Test]
        public void Test_Increment_And_Decrement_Positive_Values_Works() {
            
            var counter = CreateCounter();

            counter.Increment();
            
            Assert.That(counter.GetValue(), Is.EqualTo(1M));

            counter.Decrement();

            Assert.That(counter.GetValue(), Is.EqualTo(0M));

            counter.Increment(10M);

            Assert.That(counter.GetValue(), Is.EqualTo(10M));

            counter.Decrement(11M);

            Assert.That(counter.GetValue(), Is.EqualTo(-1M));

            counter.Increment(0.5M);

            Assert.That(counter.GetValue(), Is.EqualTo(-0.5M));

        }
        
        [Test]
        public void Test_Increment_And_Decrement_Negative_Values_Throws() {

            var counter = CreateCounter();

            Assert.That(() => counter.Increment(-1M), Throws.InstanceOf<ArgumentOutOfRangeException>());

            Assert.That(() => counter.Decrement(-1M), Throws.InstanceOf<ArgumentOutOfRangeException>());
            
        }
    }
}
