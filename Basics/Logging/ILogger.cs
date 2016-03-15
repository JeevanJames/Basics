using System;

namespace Basics.Logging
{
    public interface ILogger
    {
        void Log(LogType logType, object message, Exception exception, IFormatProvider formatProvider);

        bool IsEnabledFor(LogType logType);
    }

    public static class LoggerExtensions
    {
        public static void Debug(this ILogger logger, object message, Exception exception = null,
            IFormatProvider formatProvider = null)
        {
            logger.Log(LogType.Debug, message, exception, formatProvider);
        }

        public static void Info(this ILogger logger, object message, Exception exception = null,
            IFormatProvider formatProvider = null)
        {
            logger.Log(LogType.Info, message, exception, formatProvider);
        }

        public static void Warning(this ILogger logger, object message, Exception exception = null,
            IFormatProvider formatProvider = null)
        {
            logger.Log(LogType.Warning, message, exception, formatProvider);
        }

        public static void Error(this ILogger logger, object message, Exception exception = null,
            IFormatProvider formatProvider = null)
        {
            logger.Log(LogType.Error, message, exception, formatProvider);
        }

        public static void Fatal(this ILogger logger, object message, Exception exception = null,
            IFormatProvider formatProvider = null)
        {
            logger.Log(LogType.Fatal, message, exception, formatProvider);
        }

        public static bool IsDebugEnabled(this ILogger logger) => logger.IsEnabledFor(LogType.Debug);

        public static bool IsInfoEnabled(this ILogger logger) => logger.IsEnabledFor(LogType.Info);

        public static bool IsWarningEnabled(this ILogger logger) => logger.IsEnabledFor(LogType.Warning);

        public static bool IsErrorEnabled(this ILogger logger) => logger.IsEnabledFor(LogType.Error);

        public static bool IsFatalEnabled(this ILogger logger) => logger.IsEnabledFor(LogType.Fatal);
    }
}