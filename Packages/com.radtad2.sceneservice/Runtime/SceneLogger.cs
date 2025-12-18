using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

namespace SceneService
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class LogEntry
    {
        [UsedImplicitly] public LogLevel Level { get; }
        [UsedImplicitly] public string Message { get; }
        [UsedImplicitly] public DateTime Timestamp { get; }
        [UsedImplicitly] public string StackTrace { get; }
        
        internal LogEntry(LogLevel level, string message, string stackTrace)
        {
            Level = level;
            Message = message;
            Timestamp = DateTime.UtcNow;
            StackTrace = stackTrace;
        }
    }
    
    /// <summary>
    /// All scene logs are ported through here.
    /// </summary>
    public static class SceneLogger
    {
        /// <summary>
        /// Invoked when a log is generated.
        /// </summary>
        public static event Action<LogEntry> OnLog;
        
        private static bool _log = true;
        
        private const string Header = "[SceneService] ";
        
        /// <summary>
        /// Determines whether this logger automatically logs to the console or not. Logging events will still be
        /// fired.
        /// </summary>
        /// <param name="logImplicitly">True to allow implicit logging.</param>
        public static void SetImplicitLogging(bool logImplicitly)
        {
            _log = logImplicitly;
        }
        
        internal static void Info(string message)
        {
            message = Header + message;
            OnLog?.Invoke(new LogEntry(LogLevel.Info, message, GetCallerStackTrace()));
            if (_log) Debug.Log(message);
        }

        internal static void Warning(string message)
        {
            message = Header + message;
            OnLog?.Invoke(new LogEntry(LogLevel.Warning, message, GetCallerStackTrace()));
            if (_log) Debug.LogWarning(message);
        }

        internal static void Error(string message)
        {
            message = Header + message;
            OnLog?.Invoke(new LogEntry(LogLevel.Error, message, GetCallerStackTrace()));
            if (_log) Debug.LogError(message);
        }
        
        private static string GetCallerStackTrace(int skipFrames = 2)
        {
            // skipFrames: 0 = this method, 1 = the logger method, 2 = the caller of logger
            var stackTrace = new StackTrace(skipFrames, fNeedFileInfo: true);
            return stackTrace.ToString();
        }
    }
}