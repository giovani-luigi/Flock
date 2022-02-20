using System;

namespace Flock.Core.Utils
{
    internal static class ByteArray {

        /// <summary>
        /// Apply a XOR operation between two byte arrays of same length
        /// </summary>
        /// <exception cref="ArgumentException">Arrays are not identical length</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] Xor(byte[] seed, byte[] newValue) {
            if (seed == null) throw new ArgumentNullException(nameof(seed));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            if (seed.Length != newValue.Length)
                throw new ArgumentException("Argument arrays should have same length");

            for (int i = 0; i < seed.Length; i++) {
                seed[i] = (byte)(seed[i] ^ newValue[i]);
            }

            return seed;
        }

    }
}
