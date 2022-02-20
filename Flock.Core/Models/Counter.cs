using System;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Counters;
using Flock.Core.Network;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {
    
    /// <summary>
    /// This is a data model used to hold a single signed decimal number supporting
    /// operations to increment and decrement the value.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe by using internal synchronization.
    /// </remarks>
    public sealed class Counter : IModel<Counter> {

        private readonly object _monitor;
        private readonly Node _localNode;
        private readonly PNCounterDecimal<UniqueId> _counter;

        public event EventHandler StateChanged;

        public UniqueId ModelId { get; }

        private Counter() {
            _monitor = new object();
            _counter = new PNCounterDecimal<UniqueId>();
        }

        internal Counter(UniqueId modelId, Node localNode) : this() {
            _localNode = localNode;
            ModelId = modelId;
        }

        public void Increment(decimal value = 1M) {
            lock (_monitor) {
                _counter.Increment(_localNode.NodeId, value);
            }
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Decrement(decimal value = 1M) {
            lock (_monitor) {
                _counter.Decrement(_localNode.NodeId, value);
            }
            RaiseEventStateChanged();
        }

        public void ChangeBy(decimal value) {
            lock (_monitor) {
                if (value > 0) {
                    _counter.Increment(_localNode.NodeId, value);
                } else if (value < 0) {
                    _counter.Decrement(_localNode.NodeId, value * -1);
                } else {
                    return;
                }
            }
            RaiseEventStateChanged();
        }

        public decimal GetValue() {
            lock (_monitor) {
                return _counter.GetValue();
            }
        }
        
        /// <inheritdoc />
        public IModelState GetState() {
            lock (_monitor) {
                return new CounterState(_counter);
            }
        }

        /// <inheritdoc />
        public void Merge(IModelState state) {
            if (!(state is CounterState counterState)) return;
            lock (_monitor) {
                _counter.Merge(counterState.Counter);
            }
            RaiseEventStateChanged();
        }

        #region Implementation of IUnique

        UniqueId IUnique.UniqueId => ModelId;

        #endregion

        private void RaiseEventStateChanged() {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
