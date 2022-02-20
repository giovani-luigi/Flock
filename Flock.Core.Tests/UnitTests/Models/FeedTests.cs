using Flock.Core.Identification;
using Flock.Core.Models;
using NUnit.Framework;

namespace Flock.Core.Tests.UnitTests.Models
{
    [TestFixture]
    internal class FeedTests
    {
        
        [Test]
        public void Test_Append_And_Remove_Works() {

            var feed = new Feed<FeedStringEntry>(UniqueIdFactory.CreateNew());

            feed.Append(FeedStringEntry.NewUnique("Test"));
            
        }
        
    }
}
