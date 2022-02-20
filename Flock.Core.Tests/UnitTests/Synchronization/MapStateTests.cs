using Flock.Core.Identification;
using Flock.Core.Models;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Synchronization
{
    [TestFixture]
    internal class MapStateTests {

        [Test]
        public void Test_GetHash()
        {
            var map = new Map<MapStringEntry>(UniqueIdFactory.CreateNew());
            map.Set(MapStringEntry.NewUnique("Entry 1"));

            string hash = map.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash = " + hash);

            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void Test_Hash_Is_Different_Between_Models_When_Content_Is_Identical()
        {
            var map1 = new Map<MapStringEntry>(UniqueIdFactory.CreateNew());
            map1.Set(MapStringEntry.NewUnique("Entry 1"));
            string hash1 = map1.GetState().GetStateSignature(0).Value;

            var map2 = new Map<MapStringEntry>(UniqueIdFactory.CreateNew());
            map2.Set(MapStringEntry.NewUnique("Entry 1"));
            string hash2 = map2.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1);
            TestContext.WriteLine("Hash2 = " + hash2);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void Test_Hash_Is_Different_After_Update()
        {
            var entryKey = UniqueIdFactory.CreateNew();

            var map = new Map<MapStringEntry>(UniqueIdFactory.CreateNew());
            map.Set(new MapStringEntry(entryKey, "Entry 1"));
            string hash1 = map.GetState().GetStateSignature(0).Value;
            map.Set(new MapStringEntry(entryKey, "Entry 2"));
            string hash2 = map.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1);
            TestContext.WriteLine("Hash2 = " + hash2);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }
        
        [Test]
        public void Test_Hash_Is_Different_After_Removal()
        {
            var entryKey = UniqueIdFactory.CreateNew();

            var map = new Map<MapStringEntry>(UniqueIdFactory.CreateNew());
            map.Set(new MapStringEntry(entryKey, "Entry 1"));
            string hash1 = map.GetState().GetStateSignature(0).Value;
            map.Remove(entryKey);
            string hash2 = map.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1);
            TestContext.WriteLine("Hash2 = " + hash2);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

    }
}
