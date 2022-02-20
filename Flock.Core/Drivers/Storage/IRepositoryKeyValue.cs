using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Identification;

namespace Flock.Core.Drivers.Storage {
    public interface IRepositoryKeyValue {

        /// <summary>
        /// Load from the storage subsystem the requested value.
        /// If no entry is found with the provided <paramref name="primaryKey"/> and
        /// <paramref name="secondaryKey"/> then the returned value is null
        /// </summary>
        Task<DualKeyValue> LoadAsync(UniqueId primaryKey, UniqueId secondaryKey, CancellationToken cancellationToken);

        /// <summary>
        /// Saves into the storage subsystem the provided key-value
        /// </summary>
        /// <remarks>
        /// If a value under the same key is present in the storage, the value is replaced.
        /// This method works as a Insert OR Update.
        /// </remarks>
        Task StoreAsync(DualKeyValue value, CancellationToken cancellationToken);

    }
}
