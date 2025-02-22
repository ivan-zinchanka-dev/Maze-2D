using Maze2D.CodeBase.Logging.Contracts;
using Maze2D.CodeBase.Logging.Contracts.Generic;

namespace Maze2D.CodeBase.Logging
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