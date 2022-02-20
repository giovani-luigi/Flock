using Flock.Core.Identification;

namespace Flock.Core.Drivers.Storage {
    public class DualKeyValue : IUnique {
        
        public UniqueId PrimaryKey { get; }

        public UniqueId SecondaryKey { get; }

        public byte[] Value { get; }

        public DualKeyValue(UniqueId primaryKey, UniqueId secondaryKey, byte[] value) {
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            Value = value;
        }

        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => PrimaryKey;

        #endregion
    }
}
