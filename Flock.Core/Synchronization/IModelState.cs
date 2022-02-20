using Flock.Core.Identification;

namespace Flock.Core.Synchronization {

    public interface IModelState {
        
        bool IsEmpty { get; }

        Hash GetStateSignature(long part);

    }
}
