using System;

namespace Flock.Core.Drivers.Storage {
    public interface IStorageDriver : IDisposable {
        
        IRepositoryKeyValue KeyValues { get; }
        
    }
}
