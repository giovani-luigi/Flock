using Flock.Core.Identification;
using MessagePack;

namespace Flock.Core.Models {
    
    [MessagePackObject()]
    public class FeedStringEntry : IUnique {
        
        [Key(0)] 
        public UniqueId Key { get; set; }
        
        [Key(1)] 
        public string Value { get; set; }
        
        [SerializationConstructor]
        public FeedStringEntry(UniqueId key, string value) {
            Key = key;
            Value = value;
        }

        UniqueId IUnique.UniqueId => Key;

        #region Static

        public static FeedStringEntry NewUnique(string value) {
            return new FeedStringEntry(UniqueIdFactory.CreateNew(), value);
        }

        #endregion

    }
}
