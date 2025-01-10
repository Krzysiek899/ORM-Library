using System;
using Logging; 

public class Progrma 
{
    public static void Main(string[] args)
    {
        // Tworzymy obiekt loggera
            Logger logger = Logger.GetInstance();

            // Logujemy różne rodzaje komunikatów
            logger.LogInfo("Aplikacja uruchomiona");
            logger.LogWarning("To jest ostrzeżenie");
            try
            {
                // Symulacja błędu
                throw new InvalidOperationException("Wystąpił błąd w aplikacji");
            }
            catch (Exception ex)
            {
                logger.LogError("Wystąpił błąd", ex);
            }

            
    }
}