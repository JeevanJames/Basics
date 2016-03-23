using System;

namespace Basics.Logging
{
    internal sealed class NullLogger : ILogger
    {
        void ILogger.Log(LogType logType, object message, Exception exception, IFormatProvider formatProvider)
        {
        }

        bool ILogger.IsEnabledFor(LogType logType) => false;
    }
}