using System;
using JanZinch.Services.Logging.Contracts;

namespace JanZinch.Services.Logging.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogVerbose(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Verbose, message, args);
        }
        
        public static void LogDebug(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Debug, message, args);
        }
        
        public static void LogInformation(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Debug, message, args);
        }
        
        public static void LogWarning(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Debug, message, args);
        }
        
        public static void LogError(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Error, message, args);
        }
        
        public static void LogError(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.Log(LogEventLevel.Error, exception, message, args);
        }
        
        public static void LogFatal(this ILogger logger, string message, params object[] args)
        {
            logger.Log(LogEventLevel.Fatal, message, args);
        }
        
        public static void LogFatal(this ILogger logger, Exception exception, string message = null, params object[] args)
        {
            logger.Log(LogEventLevel.Fatal, exception, message, args);
        }
    }
}