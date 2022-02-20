using System;
using System.Collections.Generic;
using Flock.Core.Identification;
using Flock.Core.Models;

namespace Flock.Core.Synchronization {

    public class ModelCache<T> : IDisposable where T : IModel<T> {
        
        private readonly Dictionary<UniqueId, T> _model;
        private readonly Dictionary<UniqueId, ModelSynchronizer> _synchronizer;
        
        public ModelCache() {
            _model = new Dictionary<UniqueId, T>();
            _synchronizer = new Dictionary<UniqueId, ModelSynchronizer>();
        }

        public T GetModel(UniqueId modelId, Func<T> modelFactory) {
            lock (_model) {
                if (_model.TryGetValue(modelId, out var model)) {
                    return model;
                } else {
                    var newModel = modelFactory();
                    _model.Add(modelId, newModel);
                    return newModel;
                }
            }
        }

        public T GetModelOrDefault(UniqueId modelId) {
            lock (_model) {
                return _model.TryGetValue(modelId, out var model) ? (T) model : default;
            }
        }

        public ModelSynchronizer GetSynchronizer(UniqueId modelId, Func<ModelSynchronizer> synchronizerFactory) {
            lock (_synchronizer) {
                if (_synchronizer.TryGetValue(modelId, out var synchronizer)) {
                    return synchronizer;
                } else {
                    var newSynchronizer = synchronizerFactory();
                    _synchronizer.Add(modelId, newSynchronizer);
                    return newSynchronizer;
                }
            }
        }

        public ModelSynchronizer GetSynchronizerOrDefault(UniqueId modelId) {
            lock (_synchronizer) {
                return _synchronizer.TryGetValue(modelId, out var synchronizer) ? synchronizer : null;
            }
        }
        
        #region IDisposable

        /// <inheritdoc />
        public void Dispose() {
            foreach (var synchronizer in _synchronizer) {
                synchronizer.Value.Dispose();
            }
        }

        #endregion

    }
}
