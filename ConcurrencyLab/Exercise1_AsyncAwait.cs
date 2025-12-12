using System;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    public static class Exercise1_AsyncAwait
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Exercise1: Async/Await basics ===");

            int input = 1000;

            int syncResult = ComputeSumOfSquaresSync(input);
            int asyncResult = await ComputeSumOfSquaresAsync(input);
        
            ResultChecker.Check("Exercise1", syncResult, asyncResult);
            Console.WriteLine();
        }

        private static int ComputeSumOfSquaresSync(int n)
        {
            int sum = 0;
            for (int i = 1; i <= n; i++)
            {
                sum += i * i;
            }

            return sum;
        }

        // TODO (Exercise1):
        //  - Zaimplementuj asynchroniczną wersję ComputeSumOfSquaresSync.
        //  - Zwróć Task<int>:
        //      * użyj słowa kluczowego 'async' oraz 'await'
        //      * zasymuluj dłuższą operację (np. Task.Delay) przed liczeniem
        //        albo wewnątrz pętli
        //  - Nie używaj w tym zadaniu Parallel.For ani Thread.
        //
        //  Podpowiedź:
        //    - Możesz użyć:
        //        * 'Task.Run(() => { ... obliczenia ... })'
        //      albo
        //        * 'async Task<int>' + pętla + 'await Task.Delay(...)'
        
        public async static Task<int> ComputeSumOfSquaresAsync(int n)
        {
            int workers = Environment.ProcessorCount;
            var tasks = new Task<int>[workers];
            
          
            for (int i = 0; i < workers; i++)
            {
                int workerId = i;
                tasks[i] = Task.Run(() =>
                {
                    int partial_sum = 0;
                    for (int y = workerId;y <= n; y+=workers)
                    {
                        partial_sum += y * y;
                    }

                    return partial_sum;
                });
            }
            
            int[] partial_sums = await Task.WhenAll(tasks);
            
            return partial_sums.Sum();

        }
    }
}