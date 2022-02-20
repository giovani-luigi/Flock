using System.Threading;

namespace Flock.Core.Identification
{
    internal class SequentialVersionGenerator : IVersionGenerator {
        
        private long _version;

        public SequentialVersionGenerator(long initialVersion) {
            _version = initialVersion;
        }

        #region Implementation of IVersionGenerator

        /// <inheritdoc />
        public long GetNextVersion() {
            return Interlocked.Increment(ref _version);
        }

        #endregion
    }
}
