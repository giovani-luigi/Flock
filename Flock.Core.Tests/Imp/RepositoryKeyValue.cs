using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flock.Core.Drivers.Storage;
using Flock.Core.Identification;

namespace Flock.Core.Tests.Imp {
    internal class RepositoryKeyValue : IRepositoryKeyValue {

        class ByteArray {
            public byte[] Bytes;
        }

        private readonly Dictionary<UniqueId, Dictionary<UniqueId, ByteArray>> _values;

        public RepositoryKeyValue() {
            _values = new Dictionary<UniqueId, Dictionary<UniqueId, ByteArray>>();
        }
        
        #region Implementation of IRepositoryKeyValue

        /// <inheritdoc />
        public Task<DualKeyValue> LoadAsync(UniqueId primaryKey, UniqueId secondaryKey, CancellationToken cancellationToken) {
            if (!_values.TryGetValue(primaryKey, out var d1)) return null;
            if (!d1.TryGetValue(secondaryKey, out var value)) return null;
            return Task.FromResult(new DualKeyValue(primaryKey, secondaryKey, value.Bytes));
        }

        /// <inheritdoc />
        public Task StoreAsync(DualKeyValue value, CancellationToken cancellationToken) {
            if (_values.TryGetValue(value.PrimaryKey, out var d1)) {
                if (d1.TryGetValue(value.SecondaryKey, out var val)) {
                    val.Bytes = value.Value;
                } else {
                    d1.Add(value.SecondaryKey, new ByteArray { Bytes = value.Value});
                }
            } else {
                var d2 = new Dictionary<UniqueId, ByteArray> {
                    {value.SecondaryKey, new ByteArray { Bytes = value.Value}}
                };
                _values.Add(value.PrimaryKey, d2);
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}
