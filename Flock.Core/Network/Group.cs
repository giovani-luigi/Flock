using Flock.Core.Identification;
using MessagePack;

namespace Flock.Core.Network {

    [MessagePackObject()]
    public class Group : IUnique {

        [Key(0)]
        public UniqueId GroupId { get; }

        [SerializationConstructor]
        public Group(UniqueId groupId) {
            GroupId = groupId;
        }

        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => GroupId;

        #endregion

        #region Static

        public static Group World => new Group(UniqueIdFactory.Default);

        #endregion
    }
}