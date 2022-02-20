using System.Collections.Generic;
using Flock.Core.Network;

namespace Flock.Core.Synchronization {
    public class DataModelClock {

        private readonly Dictionary<Node, ClockValue> _clock;

        public DataModelClock() {
            _clock = new Dictionary<Node, ClockValue>();
        }

        public ClockValue GetNext(Node node) {
            if (_clock.TryGetValue(node, out var count)) {
                var next = ClockValue.GetNext(count);
                _clock[node] = next;
                return next;
            } else {
                _clock.Add(node, ClockValue.Empty);
                return ClockValue.Empty;
            }
        }

    }
}
