using JanZinch.Services.Logging.Contracts.Generic;

namespace JanZinch.Services.Logging.Contracts
{
    public interface ILoggerFactory
    {
        public ILogger<T> CreateLogger<T>();
    }
}