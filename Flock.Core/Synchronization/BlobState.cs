using System;
using System.Linq;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Registers;
using MessagePack;

namespace Flock.Core.Synchronization
{
    internal class BlobState : IModelState {

        #region State

        [Key(0)]
        public readonly BWRegister<byte[]> Register;

        #endregion

        #region Constructors

        [SerializationConstructor]
        public BlobState(BWRegister<byte[]> register) {
            if (register == null) throw new ArgumentNullException(nameof(register));
            Register = register.Copy();
        }

        #endregion

        #region Implementation of IModelState

        [IgnoreMember]
        public bool IsEmpty => Register.Data == null || !Register.Data.Any();

        /// <inheritdoc />
        public Hash GetStateSignature(long part) {
            return new Hash(Register.Version.ToString());
        }

        #endregion
    }
}
