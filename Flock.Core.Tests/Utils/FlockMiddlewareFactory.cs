using Flock.Core.Tests.Imp;

namespace Flock.Core.Tests.Utils {
    internal static class FlockMiddlewareFactory {

        public static FlockMiddleware CreateTestDefault() {
            return new FlockMiddleware(new FlockConfigurationDefault());
        }

    }
}
