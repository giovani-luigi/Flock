using Flock.Core.Identification;
using Flock.Core.Synchronization;

namespace Flock.Core.Serialization
{
    internal class MapStateSerializer<T> : MessagePackSerializer<MapState<T>> where T : IUnique { }
}
