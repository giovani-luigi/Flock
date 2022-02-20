using System;
using System.Collections.Generic;
using System.Linq;

namespace Flock.Core.Identification {
    public class UniqueIdFactory {

        private static Random _random;
        private static Random Random => _random ?? (_random = new Random());

        public static UniqueId Default {
            get => new UniqueId(Enumerable.Repeat<byte>(0, UniqueId.LENGTH).ToArray());
        }

        public static UniqueId CreateNew() {
            var bytes = new byte[UniqueId.LENGTH];
            Random.NextBytes(bytes);
            return new UniqueId(bytes);
        }

        public static UniqueId ParseBase64(string stringBase64) {
            return new UniqueId(Convert.FromBase64String(stringBase64));
        }

        public static UniqueId Combine(IEnumerable<UniqueId> idList) {
            // TODO: Optimize using SIMD instructions
            var bytes = new byte[UniqueId.LENGTH];
            var uniqueIds = idList as UniqueId[] ?? idList.ToArray();
            for (int i = 0; i < bytes.Length; i++) {
                foreach (var id in uniqueIds) {
                    bytes[i] ^= id.GetByte(i);
                }
            }
            return new UniqueId(bytes);
        }

    }
}
