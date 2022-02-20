using System;
using Flock.Core.Identification;
using MessagePack;

namespace Flock.Core.Models.CRDTs.Registers {

    /// <summary>
    /// Biggest-Version-Wins Register (BVWRegister)
    /// </summary>
    /// <remarks>
    /// A BVW-Register is an abstract class to build Conflict-free Replicated Data Type (CRDT) registers
    /// A register is a data type that contains a single value of type <typeparamref name="TData"/>
    /// Each modification of the replica is assigned with a value provided to be provided by a derived class.
    /// This is a generic class used to build other register CRDTs
    /// </remarks>
    /// <typeparam name="TData">
    /// The type of the data carried by this register
    /// </typeparam>
    [MessagePackObject()]
    public sealed class BWRegister<TData> : ICrdt<BWRegister<TData>> {
        
        private IVersionGenerator _versionGenerator;

        #region State

        [Key(0)]
        public long Version { get; private set; }
        
        [Key(1)]
        public TData Data { get; private set; }

        #endregion

        public IVersionGenerator VersionGenerator {
            get => _versionGenerator = _versionGenerator ?? new SequentialVersionGenerator(0);
            set => _versionGenerator = value ?? throw new ArgumentNullException();
        }

        #region Constructors

        [SerializationConstructor]
        public BWRegister(long version, TData data) {
            Version = version;
            Data = data;
        }

        public BWRegister() : this(0, default) {
        }

        #endregion

        #region Public Methods

        public void Set(TData data) {
            Data = data;
            Version = VersionGenerator.GetNextVersion();
        }

        public TData Get() {
            return Data;
        }
        
        #endregion

        #region Implementation of ICrdt<BWRegister<TData>>

        /// <inheritdoc />
        public void Merge(BWRegister<TData> other) {
            // if 'other' is newer, use its data and timestamp
            if (Version.CompareTo(other.Version) < 0) { 
                Data = other.Data;
                Version = other.Version;
            } 
        }

        /// <inheritdoc />
        public BWRegister<TData> Copy() {
            return new BWRegister<TData>(this.Version, this.Data);
        }

        #endregion
        
    }
}
