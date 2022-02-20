using System;

namespace Flock.Core.Identification
{
    internal class DateTimeVersionGenerator : IVersionGenerator 
    {

        private IDateTimeProvider _dateTimeProvider;

        public DateTimeVersionGenerator(IDateTimeProvider dateTimeProvider = null) {
            _dateTimeProvider = dateTimeProvider ?? new SystemDateTimeProvider();
        }

        #region Implementation of IVersionGenerator

        /// <inheritdoc />
        public long GetNextVersion() 
        {
            return DateTime.UtcNow.Ticks;
        }

        #endregion
    }
}