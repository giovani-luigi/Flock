namespace Flock.Core.Synchronization {
    public readonly struct ClockValue {

        public readonly int Count;

        public ClockValue(int count) {
            Count = count;
        }
        
        public static ClockValue Empty {
            get => new ClockValue(0);
        }

        public static ClockValue GetNext(in ClockValue current) {
            return new ClockValue(current.Count + 1);
        }
    }
}
