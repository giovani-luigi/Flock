using System;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Collections;
using Flock.Core.Network;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {

    /// <summary>
    /// Collection of spatially (space-time) unique entries
    /// This data model is intended for applications using geolocation data.
    /// </summary>
    public class SpatialLog<T> : IModel<SpatialLog<T>> where T : ISpatiallyUnique {
        
        private readonly Node _localNode;
        private readonly object _monitor;

        private GSet<T> _gset;

        public UniqueId ModelId { get; }

        private SpatialLog() {
            _monitor = new object();
            _gset = new GSet<T>();
        }

        public SpatialLog(UniqueId modelId, Node localNode) : this() {
            _localNode = localNode;
            ModelId = modelId;
        }
        
        #region Public Methods

        public void Add(T item) {
            throw new NotImplementedException();
            //lock (_monitor) {

            //}
            //RaiseEventStateChanged();
        }

        #endregion

        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => ModelId;

        #endregion

        /// <inheritdoc />
        public event EventHandler StateChanged;

        public IModelState GetState() {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Merge(IModelState state) {
            throw new NotImplementedException();
        }

        private void RaiseEventStateChanged() {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
