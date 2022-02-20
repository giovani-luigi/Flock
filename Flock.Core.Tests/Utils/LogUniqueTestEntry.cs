using Flock.Core.Identification;

namespace Flock.Core.Tests.Utils {
    internal class LogUniqueTestEntry : IUnique {
        
        public UniqueId UniqueId { get; }
        
        public int Data { get; }

        public LogUniqueTestEntry(UniqueId uniqueId, int data) {
            UniqueId = uniqueId;
            Data = data;
        }

    }
}
