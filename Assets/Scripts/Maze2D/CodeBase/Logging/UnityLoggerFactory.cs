using Maze2D.CodeBase.Logging.Contracts;
using Maze2D.CodeBase.Logging.Contracts.Generic;

namespace Maze2D.CodeBase.Logging
{
    public class UnityLoggerFactory : ILoggerFactory
    {
        public ILogger<T> CreateLogger<T>()
        {
            return new UnityLogger<T>();
        }
    }
}