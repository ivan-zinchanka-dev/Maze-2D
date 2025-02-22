using Maze2D.CodeBase.Logging.Contracts.Generic;

namespace Maze2D.CodeBase.Logging.Contracts
{
    public interface ILoggerFactory
    {
        public ILogger<T> CreateLogger<T>();
    }
}