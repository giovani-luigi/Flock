using Flock.Core.Identification;
using Flock.Core.Synchronization;

namespace Flock.Core.Serialization
{
    internal class FeedStateSerializer<T> : MessagePackSerializer<FeedState<T>> where T : IUnique { }
}
