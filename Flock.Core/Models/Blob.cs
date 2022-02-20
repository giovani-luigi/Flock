using System;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Registers;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {

    public class Blob : IModel<Blob> {

        private readonly object _monitor;
        private readonly BWRegister<byte[]> _register;

        public event EventHandler StateChanged;

        public UniqueId ModelId { get; }

        private Blob() {
            _monitor = new object();
            _register = new BWRegister<byte[]> {
                VersionGenerator = new DateTimeVersionGenerator()
            };
        }

        internal Blob(UniqueId modelId) : this() {
            ModelId = modelId;
        }

        public void Set(byte[] data) {
            lock (_monitor) {
                _register.Set(data);
            }
        }

        public byte[] Get() {
            lock (_monitor) {
                return _register.Get();
            }
        }

        #region Implementation of IUnique

        /// <inheritdoc />
        UniqueId IUnique.UniqueId => ModelId;

        #endregion

        /// <inheritdoc />
        public IModelState GetState() {
            lock (_monitor) {
                return new BlobState(_register);
            }
        }

        /// <inheritdoc />
        public void Merge(IModelState state) {
            if (!(state is BlobState blobState)) return;
            lock (_monitor) {
                _register.Merge(blobState.Register);
            }
            RaiseEventStateChanged();
        }
        
        private void RaiseEventStateChanged() {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
