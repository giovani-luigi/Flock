namespace Flock.Core.Drivers.Network {
    public enum PacketType : byte {
        Unknown = 0,
        CounterQuery = 101,
        CounterSeed = 103,
        FeedQuery = 111,
        FeedSeed = 113,
        BlobQuery = 121,
        BlobSeed = 123,
        MapQuery = 131,
        MapSeed = 133,
    }
}
