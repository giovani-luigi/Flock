using Flock.Core.Identification;
using Flock.Core.Models;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Synchronization
{
    [TestFixture]
    internal class BlobStateTests
    {
        [Test]
        public void Test_GetHash()
        {
            var blob = new Blob(UniqueIdFactory.CreateNew());

            var data = new byte[] { 0x01, 0x02 };

            blob.Set(data);

            string hash = blob.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash = " + hash);

            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public void Test_GetHash_Are_Different_For_Different_Models_With_Same_Data_But_Different_Time()
        {
            byte[] data = { 0x01, 0x02 };

            var blob1 = new Blob(UniqueIdFactory.CreateNew());
            blob1.Set(data);
            
            string hash1 = blob1.GetState().GetStateSignature(0).Value;

            var blob2 = new Blob(UniqueIdFactory.CreateNew());
            blob2.Set(data);
            string hash2 = blob2.GetState().GetStateSignature(0).Value;

            TestContext.WriteLine("Hash1 = " + hash1 + "\nHash2 = " + hash2);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }
        
    }
}
