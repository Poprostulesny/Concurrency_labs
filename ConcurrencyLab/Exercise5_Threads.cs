using System;
using System.Threading;

namespace ConcurrencyLab
{
    public static class Exercise5_Threads
    {
        public static void Run()
        {
            Console.WriteLine("=== Exercise5: Threads ===");

            int iterations = 10_000;
            int expected = iterations;
            int actual = RunThreadAndReturnCounter(iterations);

            ResultChecker.Check("Exercise5", expected, actual);
            Console.WriteLine();
        }

        // TODO (Exercise5):
        //  - Utwórz zmienną 'counter' i ustaw ją na 0.
        //  - Utwórz nowy Thread (System.Threading.Thread), który:
        //        - w pętli 'iterations' razy zwiększy zmienną 'counter'
        //  - Uruchom wątek (Start).
        //  - W głównym wątku poczekaj na zakończenie tego wątku (Join).
        //  - Na końcu zwróć wartość 'counter'.
        //
        //  Ograniczenia:
        //    - Nie używaj w tym zadaniu Task, async/await, Parallel.For, ThreadPool itp.
        //
        //  Podpowiedź:
        //    - Możesz użyć lambdy:
        //          var thread = new Thread(() =>
        //          {
        //              // tutaj pętla inkrementująca counter
        //          });
        public static int RunThreadAndReturnCounter(int iterations)
        {
            int counter = 0;
            Thread t = new Thread(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    counter++;
                }
            });
            t.Start();
            t.Join();
            return counter;
        }
    }
}