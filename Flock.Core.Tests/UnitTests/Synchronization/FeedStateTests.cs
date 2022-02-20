using Flock.Core.Identification;
using Flock.Core.Models;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Synchronization
{
    [TestFixture]
    public class FeedStateTests
    {

        [Test]
        public void Test_GetHash()
        {
            var feed = new Feed<FeedStringEntry>(UniqueIdFactory.CreateNew());
            feed.Append(new FeedStringEntry(UniqueIdFactory.CreateNew(), "Item 1"));
            string hash = feed.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash = " + hash);

            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void Test_GetHash_Result_Different_For_Different_States()
        {
            var feed = new Feed<FeedStringEntry>(UniqueIdFactory.CreateNew());
            var key1 = UniqueIdFactory.CreateNew();
            var key2 = UniqueIdFactory.CreateNew();
            
            feed.Append(new FeedStringEntry(key1, "Item 1"));
            string hash1 = feed.GetState().GetStateSignature(0).Value;

            feed.Append(new FeedStringEntry(key2, "Item 2"));
            string hash2 = feed.GetState().GetStateSignature(0).Value;

            feed.Remove(key1);
            string hash3 = feed.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1);
            TestContext.WriteLine("Hash2 = " + hash2);
            TestContext.WriteLine("Hash3 = " + hash3);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
            Assert.That(hash2, Is.Not.EqualTo(hash3));
            Assert.That(hash3, Is.Not.EqualTo(hash1));
        }

    }
}
