using JanZinch.Services.Logging.Contracts;
using JanZinch.Services.Logging.Contracts.Generic;

namespace JanZinch.Services.Logging.Standard
{
    public class UnityLoggerFactory : ILoggerFactory
    {
        private readonly LogEventLevel _minimumLevel;

        public UnityLoggerFactory(LogEventLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;
        }

        public ILogger<T> CreateLogger<T>()
        {
            return new UnityLogger<T>(_minimumLevel);
        }
    }
}