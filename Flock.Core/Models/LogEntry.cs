using Flock.Core.Identification;
using Flock.Core.Network;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {
    
    internal class LogEntry<T> where T : IUnique {
        
        private readonly Node _node;
        private readonly ClockValue _clockValue;

        public T Data { get; }

        public LogEntry(Node node, ClockValue clockValue, T data) {
            _node = node;
            _clockValue = clockValue;
            Data = data;
        }

    }
}
