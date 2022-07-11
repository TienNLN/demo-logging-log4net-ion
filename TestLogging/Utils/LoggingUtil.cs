using System;
using log4net;
using log4net.Core;

namespace TestLogging.Utils
{
    public static class LoggingUtil
    {
        private static readonly ILog _applicationLogger = LogManager.GetLogger("ApplicationLogger");

        public static void Info(object message)
        {
            _applicationLogger.Info("Log" + Environment.NewLine
                                          + "=========================" + Environment.NewLine
                                          + "TraceId: " + LogicalThreadContext.Properties["TraceId"] + Environment.NewLine
                                          + "Message: " + message + Environment.NewLine
                                          + "=========================");
        }
    }
}