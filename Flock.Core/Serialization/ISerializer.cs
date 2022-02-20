namespace Flock.Core.Serialization {
    internal interface ISerializer<T> {

        byte[] Serialize(T instance);

        T Deserialize(byte[] bytes);

    }
}
