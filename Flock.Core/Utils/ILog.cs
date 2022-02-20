using System;

namespace Flock.Core.Utils {
    public interface ILog {

        void Info(string message);

        void Warning(string message);

        void Error(string message, Exception exception = null);

    }
}
