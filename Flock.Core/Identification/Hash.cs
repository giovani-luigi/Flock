using System;
using MessagePack;

namespace Flock.Core.Identification {
    
    [MessagePackObject()]
    public readonly struct Hash : IEquatable<Hash> {
        
        #region Serialization

        [Key(0)]
        public string Value { get; }

        public static Hash Empty => new Hash(string.Empty);

        #endregion

        [SerializationConstructor()]
        public Hash(string value) {
            Value = value;
        }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Hash other) {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is Hash other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(Hash left, Hash right) {
            return left.Equals(right);
        }

        public static bool operator !=(Hash left, Hash right) {
            return !left.Equals(right);
        }
        
        #endregion

        #region Overrides of ValueType

        /// <inheritdoc />
        public override string ToString() {
            return Value;
        }

        #endregion

    }
}
