﻿using System;

namespace Maze2D.CodeBase.Logging.Contracts
{
    public interface ILogger
    {
        public void Log(LogEventLevel logLevel, Exception exception, string message = null, params object[] args);
        public void Log(LogEventLevel logLevel, string message, params object[] args);
    }
}