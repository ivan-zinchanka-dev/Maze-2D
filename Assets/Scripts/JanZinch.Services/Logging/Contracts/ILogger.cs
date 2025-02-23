using System;

namespace JanZinch.Services.Logging.Contracts
{
    public interface ILogger
    {
        public void Log(LogEventLevel logLevel, Exception exception, string message = null, params object[] args);
        public void Log(LogEventLevel logLevel, string message, params object[] args);
    }
}