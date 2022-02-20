using System;
using Flock.Core.Utils;
using NUnit.Framework;
using Environment = System.Environment;

namespace Flock.Core.Tests.Utils {
    public class TestLogger : ILog {

        public TestLogger() {
        }
        
        #region Implementation of ILog

        /// <inheritdoc />
        public void Info(string message) {
            TestContext.WriteLine(message);
        }

        /// <inheritdoc />
        public void Warning(string message) {
            TestContext.WriteLine(message);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception = null) {
            TestContext.WriteLine(message + Environment.NewLine + exception);
        }

        #endregion
    }
}
