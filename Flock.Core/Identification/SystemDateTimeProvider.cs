using System;

namespace Flock.Core.Identification
{
    public class SystemDateTimeProvider : IDateTimeProvider {
        public DateTime TimeUtcNow => DateTime.UtcNow;
    }
}
