using System.Text;
using Flock.Core.Identification;
using Flock.Core.Models.CRDTs.Counters;
using Flock.Core.Utils;
using MessagePack;

namespace Flock.Core.Synchronization {

    [MessagePackObject()]
    public class CounterState : IModelState {
        
        #region State

        [Key(0)]
        public readonly PNCounterDecimal<UniqueId> Counter;

        #endregion

        #region Constructors

        [SerializationConstructor]
        public CounterState(PNCounterDecimal<UniqueId> counter) {
            Counter = counter.Copy();
        }

        #endregion

        #region Implementation of IModelState

        [IgnoreMember]
        public bool IsEmpty => Counter.DecrementValue == decimal.Zero && Counter.IncrementValue == decimal.Zero;

        /// <inheritdoc />
        public Hash GetStateSignature(long part) {
            var incs = decimal.GetBits(Counter.IncrementValue);
            var decs = decimal.GetBits(Counter.DecrementValue);
            var hash = new StringBuilder(4 * 8 * 2);
            for (int i = 0; i < 4; i++) {
                hash.Append(incs[i].ToString("X8"));
                hash.Append(decs[i].ToString("X8"));
            }
            return new Hash(RunLengthEncoding.Encode(hash.ToString()));
        }

        #endregion
    }
}
