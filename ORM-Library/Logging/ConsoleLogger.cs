using System;

namespace ORMLibrary.Logging
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"INFO: {message}");
            Console.ResetColor();
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {message}");
            Console.ResetColor();
        }

        public void LogError(string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message} - Exception: {ex.Message}");
            Console.ResetColor();
        }
    }
}
