using System;

namespace Flock.Core.Identification {
    public interface IDateTimeProvider {

        DateTime TimeUtcNow { get; }

    }
}