using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Flock.Core.Identification {

    /// <summary>
    /// Represents a 16 BYTES (128 bits) Identifier 
    /// </summary>
    [MessagePackObject()]
    public sealed class UniqueId : IEquatable<UniqueId> {
        
        public const byte LENGTH = 16;

        #region State

        /// <summary>
        /// The sequence of bytes that form the ID (little-endian, i.e. the first byte is the least significant)
        /// </summary>
        [Key(0)] 
        private readonly byte[] _bytes;

        #endregion
        
        #region Constructors

        private UniqueId() { } // internal/serializer ctor

        [SerializationConstructor]
        internal UniqueId(byte[] bytes) {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length != LENGTH)
                throw new ArgumentOutOfRangeException(nameof(bytes), $"Expected {LENGTH} bytes");
            _bytes = bytes;
        }
        
        #endregion

        #region Public Methods

        public IReadOnlyCollection<byte> GetBytes() {
            return new ReadOnlyCollection<byte>(_bytes);
        }

        /// <summary>
        /// Gets the byte at the given index. The bytes are ALWAYS stored in little-endian,
        /// i.e. the first byte (index=0) is the least significant.
        /// </summary>
        public byte GetByte(int index) {
            if (index >= LENGTH || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            return _bytes[index];
        }

        public byte GetLastByte() => GetByte(LENGTH - 1);

        public byte GetFirstByte() => GetByte(0);

        public Hash ToHash() => new Hash(ToString());

        #endregion

        #region IEquatable / Equals
        
        [IgnoreMember] // remove from serialization
        private int _hash; // buffer

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() {
            if (_hash != 0) return _hash; // already calculated
            int b1 = _bytes[0] ^ _bytes[4] ^ _bytes[8] ^ _bytes[12];
            int b2 = _bytes[1] ^ _bytes[5] ^ _bytes[9] ^ _bytes[13];
            int b3 = _bytes[2] ^ _bytes[6] ^ _bytes[10] ^ _bytes[14];
            int b4 = _bytes[3] ^ _bytes[7] ^ _bytes[11] ^ _bytes[15];
            _hash = 0 | (b1 << 0) | (b2 << 8) | (b3 << 16) | (b4 << 24);
            return _hash;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) {
            return obj is UniqueId id && Equals(id);
        }

        public bool Equals(UniqueId other) {
            return other != null &&
                   _bytes[00] == other._bytes[00] &&
                   _bytes[01] == other._bytes[01] &&
                   _bytes[02] == other._bytes[02] &&
                   _bytes[03] == other._bytes[03] &&
                   _bytes[04] == other._bytes[04] &&
                   _bytes[05] == other._bytes[05] &&
                   _bytes[06] == other._bytes[06] &&
                   _bytes[07] == other._bytes[07] &&
                   _bytes[08] == other._bytes[08] &&
                   _bytes[09] == other._bytes[09] &&
                   _bytes[10] == other._bytes[10] &&
                   _bytes[11] == other._bytes[11] &&
                   _bytes[12] == other._bytes[12] &&
                   _bytes[13] == other._bytes[13] &&
                   _bytes[14] == other._bytes[14] &&
                   _bytes[15] == other._bytes[15];
        }

        public static bool operator ==(UniqueId left, UniqueId right) {
            if (left is null) {
                if (right is null) return true;
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(UniqueId left, UniqueId right) {
            return !(left == right);
        }

        #endregion

        #region Overrides of Object
        
        /// <summary>
        /// Convert the ID to a base64 string
        /// </summary>
        public override string ToString() {
            return Convert.ToBase64String(_bytes, Base64FormattingOptions.None);
        }

        #endregion
        
    }
}
