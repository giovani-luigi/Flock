using System;
using System.Threading;
using Flock.Core.Drivers.Network;
using Flock.Core.Identification;
using Flock.Core.Models;
using Flock.Core.Synchronization;

namespace Flock.Core.Network {
    public class Client : IDisposable {

        private readonly CancellationTokenSource _cancelSource;

        private readonly ModelCache<Counter> _counters;
        private readonly ModelCache<Blob> _blobs;
        private readonly ModelCache<Feed<FeedStringEntry>> _feeds;
        private readonly ModelCache<Map<MapStringEntry>> _maps;

        internal FlockMiddleware Middleware { get; }

        public Node Node { get; }

        public Group Group { get; }

        internal Client(FlockMiddleware middleware, Node node, Group group) {
            Middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Group = group ?? throw new ArgumentNullException(nameof(group));
            _cancelSource = new CancellationTokenSource();
            
            Middleware.Network.PacketReceived += NetworkOnPacketReceived;

            // create dictionary for each supported model, so we store the synchronization object
            _counters = new ModelCache<Counter>();
            _blobs = new ModelCache<Blob>();
            _feeds = new ModelCache<Feed<FeedStringEntry>>();
            _maps = new ModelCache<Map<MapStringEntry>>();
        }
        
        #region Public User API

        public Counter GetCounter(UniqueId modelId) {
            var counter = GetCounterInstance(modelId);
            var sync = (CounterSynchronizer)_counters.GetSynchronizer(modelId, () => new CounterSynchronizer(this, counter));
            sync.QueryAsync(); // query network to see if some node has any update
            return counter;
        }

        public Blob GetBlob(UniqueId modelId) {
            var blob = GetBlobInstance(modelId);
            var sync = (BlobSynchronizer)_blobs.GetSynchronizer(modelId, () => new BlobSynchronizer(this, blob));
            sync.QueryAsync(); // query network to see if some node has any update
            return blob;
        }

        public Feed<FeedStringEntry> GetFeed(UniqueId modelId) {
            var feed = GetFeedInstance(modelId);
            var sync = (FeedSynchronizer<FeedStringEntry>)_feeds.GetSynchronizer(modelId, () => new FeedSynchronizer<FeedStringEntry>(this, feed));
            sync.QueryAsync(); // query network to see if some node has any update
            return feed;
        }

        public Map<MapStringEntry> GetMap(UniqueId modelId) {
            var map = GetMapInstance(modelId);
            var sync = (MapSynchronizer<MapStringEntry>)_maps.GetSynchronizer(modelId, () => new MapSynchronizer<MapStringEntry>(this, map));
            sync.QueryAsync(); // query network to see if some node has any update
            return map;
        }


        #endregion

        private Counter GetCounterInstance(UniqueId modelId) {
            return _counters.GetModel(modelId, () => {
                var counter = new Counter(modelId, Node);
                counter.StateChanged += CounterOnStateChanged;
                return counter;
            });
        }

        private Feed<FeedStringEntry> GetFeedInstance(UniqueId modelId) {
            return _feeds.GetModel(modelId, () => {
                var feed = new Feed<FeedStringEntry>(modelId);
                feed.StateChanged += FeedOnStateChanged;
                return feed;
            });
        }

        private Map<MapStringEntry> GetMapInstance(UniqueId modelId) {
            return _maps.GetModel(modelId, () => {
                var map = new Map<MapStringEntry>(modelId);
                map.StateChanged += CounterOnStateChanged;
                return map;
            });
        }

        private Blob GetBlobInstance(UniqueId modelId) {
            return _blobs.GetModel(modelId, () => {
                var blob = new Blob(modelId);
                blob.StateChanged += BlobOnStateChanged;
                return blob;
            });
        }

        private void NetworkOnPacketReceived(object sender, PacketReceivedEventArgs e) {
            var packet = e.Packet;
            if (packet.Group != Group) return; // ignore packets of different groups

            try {
                switch (packet.PacketType) {
                    case PacketType.CounterQuery: // seed network if we have a counter with that ID 
                    {
                        var sync = (CounterSynchronizer)_counters.GetSynchronizer(packet.ModelId, () => new CounterSynchronizer(this, GetCounterInstance(packet.ModelId)));
                        sync.SeedAsync();
                        break;
                    }
                    case PacketType.CounterSeed: // merge with local, then forward the seed to the adjacent nodes
                    {
                        var sync = (CounterSynchronizer)_counters.GetSynchronizer(packet.ModelId, () => new CounterSynchronizer(this, GetCounterInstance(packet.ModelId)));
                        if (sync.GetCurrentStateSignature(packet.Part) != packet.StateSignature) {
                            sync.MergeAsync(packet); // this will trigger state changed which in turn will store and seed network
                        }
                        break;
                    }
                    case PacketType.FeedQuery: // seed network if we have a feed with that ID 
                    {
                        var sync = (FeedSynchronizer<FeedStringEntry>)_feeds.GetSynchronizer(packet.ModelId, () => new FeedSynchronizer<FeedStringEntry>(this, GetFeedInstance(packet.ModelId)));
                        sync.SeedAsync();
                        break;
                    }
                    case PacketType.FeedSeed: // merge with local, then forward the seed to the adjacent nodes
                    {
                        var sync = (FeedSynchronizer<FeedStringEntry>)_feeds.GetSynchronizer(packet.ModelId, () => new FeedSynchronizer<FeedStringEntry>(this, GetFeedInstance(packet.ModelId)));
                        if (sync.GetCurrentStateSignature(packet.Part) != packet.StateSignature) {
                            sync.MergeAsync(packet); // this will trigger state changed which in turn will store and seed network
                        }
                        break;
                    }
                    case PacketType.BlobQuery: // seed network if we have a blob with that ID 
                    {
                        var sync = (BlobSynchronizer)_feeds.GetSynchronizer(packet.ModelId, () => new BlobSynchronizer(this, GetBlobInstance(packet.ModelId)));
                        sync.SeedAsync();
                        break;
                    }
                    case PacketType.BlobSeed: // merge with local, then forward the seed to the adjacent nodes
                    {
                        var sync = (BlobSynchronizer)_blobs.GetSynchronizer(packet.ModelId, () => new BlobSynchronizer(this, GetBlobInstance(packet.ModelId)));
                        if (sync.GetCurrentStateSignature(packet.Part) != packet.StateSignature) {
                            sync.MergeAsync(packet); // this will trigger state changed which in turn will store and seed network
                        }
                        break;
                    }
                    case PacketType.MapQuery: // seed network if we have a map with that ID 
                    {
                        var sync = (MapSynchronizer<MapStringEntry>)_maps.GetSynchronizer(packet.ModelId, () => new MapSynchronizer<MapStringEntry>(this, GetMapInstance(packet.ModelId)));
                        sync.SeedAsync();
                        break;
                    }
                    case PacketType.MapSeed: // merge with local, then forward the seed to the adjacent nodes
                    {
                        var sync = (MapSynchronizer<MapStringEntry>)_maps.GetSynchronizer(packet.ModelId, () => new MapSynchronizer<MapStringEntry>(this, GetMapInstance(packet.ModelId)));
                        if (sync.GetCurrentStateSignature(packet.Part) != packet.StateSignature) {
                            sync.MergeAsync(packet); // this will trigger state changed which in turn will store and seed network
                        }
                        break;
                    }
                }
            } catch (OperationCanceledException) {
                // suppress cancellation
            }
        }

        private void CounterOnStateChanged(object sender, EventArgs args) {
            try {
                var counter = (Counter) sender;
                var sync = _counters.GetSynchronizer(counter.ModelId, () => new CounterSynchronizer(this, counter));
                sync.PersistAsync();
                sync.SeedAsync();
            } catch (OperationCanceledException) {
                // supress cancellation
            } catch (Exception e) {
                Middleware.Log.Error("Error seeding network and storing data, on counter state changed.", e);
            }
        }
        
        private void FeedOnStateChanged(object sender, EventArgs args) {
            try {
                var feed = (Feed<FeedStringEntry>)sender;
                var sync = _feeds.GetSynchronizer(feed.ModelId, () => new FeedSynchronizer<FeedStringEntry>(this, feed));
                sync.PersistAsync();
                sync.SeedAsync();
            } catch (OperationCanceledException) {
                // supress cancellation
            } catch (Exception e) {
                Middleware.Log.Error("Error seeding network and storing data, on counter state changed.", e);
            }
        }

        private void BlobOnStateChanged(object sender, EventArgs args) {
            try {
                var blob = (Blob)sender;
                var sync = _feeds.GetSynchronizer(blob.ModelId, () => new BlobSynchronizer(this, blob));
                sync.PersistAsync();
                sync.SeedAsync();
            } catch (OperationCanceledException) {
                // supress cancellation
            } catch (Exception e) {
                Middleware.Log.Error("Error seeding network and storing data, on counter state changed.", e);
            }
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {

            // dispose all cached object
            _counters.Dispose();

            Middleware.Network.PacketReceived -= NetworkOnPacketReceived;

            _cancelSource.Cancel();
            _cancelSource.Dispose();
        }

        #endregion

    }
}
