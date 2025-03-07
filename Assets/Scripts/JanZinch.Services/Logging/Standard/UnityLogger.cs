﻿using System;
using System.Text;
using JanZinch.Services.Logging.Contracts.Generic;
using UnityEngine;

namespace JanZinch.Services.Logging.Standard
{
    public class UnityLogger<TCategory> : ILogger<TCategory>
    {
        private readonly LogEventLevel _minimumLevel;

        public UnityLogger(LogEventLevel minimumLevel = LogEventLevel.Verbose)
        {
            _minimumLevel = minimumLevel;
        }

        public void Log(LogEventLevel logLevel, string message, params object[] args)
        {
            Log(logLevel, null, message, args);
        }
        
        public void Log(LogEventLevel logLevel, Exception exception, string message, params object[] args)
        {
            if (logLevel >= _minimumLevel)
            {
                string builtMessage = BuildMessage(logLevel, exception, message, args);
                Debug.unityLogger.Log(ToLogType(logLevel), builtMessage);
            }
        }
        
        private string BuildMessage(LogEventLevel logLevel, Exception exception, string message, params object[] args)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append($"[{logLevel}] <{typeof(TCategory).FullName}> ");
            
            bool hasMessage = !string.IsNullOrEmpty(message);
            
            if (hasMessage)
            {
                messageBuilder.Append(args.Length > 0 ? string.Format(message, args) : message);
            }

            if (exception != null)
            {
                messageBuilder.Append(hasMessage ? $"\n{exception}" : exception);
            }

            return messageBuilder.ToString();
        }
        
        private LogType ToLogType(LogEventLevel logEventLevel)
        {
            return logEventLevel switch
            {
                LogEventLevel.Verbose => LogType.Log,
                LogEventLevel.Debug => LogType.Log,
                LogEventLevel.Information => LogType.Log,
                LogEventLevel.Warning => LogType.Warning,
                LogEventLevel.Error => LogType.Error,
                LogEventLevel.Fatal => LogType.Error,
                _ => LogType.Log
            };
        }
    }
}