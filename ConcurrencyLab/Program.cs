using System;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("=== C# Concurrency Lab ===");
            Console.WriteLine();

            await Exercise1_AsyncAwait.RunAsync();
            Exercise2_TasksAndWhenAll.Run();
            Exercise3_ParallelFor.Run();
            await Exercise4_ProgressAndCancellation.RunAsync();
            Exercise5_Threads.Run();

            // NOWE:
            await Exercise6_PrimeSearch.RunAsync();

            Console.WriteLine();
            Console.WriteLine("Koniec. Sprawdź powyżej, które zadania mają OK / FAIL.");
            Console.ReadKey();
        }
    }

}