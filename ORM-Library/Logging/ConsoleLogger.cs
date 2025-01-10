using System;

namespace Logging
{
    public sealed class Logger
    {

        private Logger(){}

        private static Logger _instance;

        public static Logger GetInstance()
        {
            if(_instance == null)
            {
                _instance = new Logger();
            }
            return _instance;
        }


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
