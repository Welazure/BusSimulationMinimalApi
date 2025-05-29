namespace BusSimulationMinimal.utils;

public class Singleton
{
    private static Singleton instance;

    private static readonly object lockObject = new();

    private Singleton()
    {
        // Private constructor to prevent instantiation
    }

    public static Singleton getInstance()
    {
        if (instance == null)
            lock (lockObject)
            {
                if (instance == null) instance = new Singleton();
            }

        return instance;
    }
}