using MessagePack;

namespace Flock.Core.Serialization {
    internal class MessagePackSerializer<T> : ISerializer<T> {

        private MessagePackSerializerOptions GetOptions() {
            return MessagePackSerializerOptions.Standard.WithResolver(MessagePack.Resolvers.StandardResolverAllowPrivate.Instance);
        }

        #region Implementation of ISerializer<T>

        /// <inheritdoc />
        public byte[] Serialize(T instance) {
            return MessagePackSerializer.Serialize(instance, GetOptions());
        }

        /// <inheritdoc />
        public T Deserialize(byte[] bytes) {
            return MessagePackSerializer.Deserialize<T>(bytes, GetOptions());
        }

        #endregion
    }
}
