using System;

namespace Logging
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }

        public void LogError(string message, Exception ex)
        {
            Console.WriteLine($"ERROR: {message} - Exception: {ex.Message}");
        }
    }
}