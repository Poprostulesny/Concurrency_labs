using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    public static class Exercise6_PrimeSearch
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Exercise6: Heavy prime search (async/await + tasks + cancellation) ===");

            int from = 2;
            int to = 2_000_000_0;
            int chunkSize = 200_000;

            // 1. WERSJA SEKWENCYJNA – baseline
            var swSequential = Stopwatch.StartNew();
            int sequentialCount = CountPrimesSequential(from, to);
            swSequential.Stop();

            Console.WriteLine($"Sequential: {sequentialCount} primes, time: {swSequential.ElapsedMilliseconds} ms");

            // 2. WERSJA RÓWNOLEGŁA – Twoja implementacja
            var swParallel = Stopwatch.StartNew();
            int parallelCount = await CountPrimesParallelAsync(from, to, chunkSize, CancellationToken.None);
            swParallel.Stop();
            ResultChecker.Check("Exercise6 (prime count)", sequentialCount, parallelCount);
            Console.WriteLine($"Parallel:   {parallelCount} primes, time: {swParallel.ElapsedMilliseconds} ms");

            if (swParallel.ElapsedTicks > 0)
            {
                double speedup = (double)swSequential.ElapsedTicks / swParallel.ElapsedTicks;
                Console.WriteLine($"Speedup (seq / par): {speedup:F2}x");
            }

            // // 3. TEST ANULOWANIA – czy reagujesz na CancellationToken w CPU-bound pętli
            // var cts = new CancellationTokenSource();
            // cts.CancelAfter(100); // anuluj po ~100 ms
            //
            // bool cancelled = false;
            // try
            // {
            //     // trochę większy zakres, żeby anulowanie miało sens
            //     await CountPrimesParallelAsync(from, to * 2, chunkSize, cts.Token);
            // }
            // catch (OperationCanceledException)
            // {
            //     cancelled = true;
            // }
            //
            // ResultChecker.CheckCondition(
            //     "Exercise6 (cancellation)",
            //     cancelled,
            //     okMessage: "task observes CancellationToken",
            //     failMessage: "task did NOT react to CancellationToken");

            Console.WriteLine();
        }

        // --- WERSJA SEKWENCYJNA (gotowa, nie ruszasz tego) --------------------

        private static int CountPrimesSequential(int fromInclusive, int toInclusive)
        {
            int count = 0;
            for (int n = fromInclusive; n <= toInclusive; n++)
            {
                if (IsPrime(n))
                    count++;
            }

            return count;
        }

        private static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int limit = (int)Math.Sqrt(n);
            for (int d = 3; d <= limit; d += 2)
            {
                if (n % d == 0)
                    return false;
            }

            return true;
        }

        // --- TWOJE ZADANIE ---------------------------------------------------

        // TODO (Exercise6):
        //  Zaimplementuj równoległe liczenie liczb pierwszych w zakresie
        //  [fromInclusive, toInclusive] przy użyciu:
        //
        //    - async/await
        //    - Task.Run
        //    - Task.WhenAll
        //    - CancellationToken (CPU-bound)
        //
        //  Wymagania:
        //    1. Podziel zakres [fromInclusive, toInclusive] na mniejsze kawałki
        //       (chunki) długości 'chunkSize'.
        //       Przykład:
        //         [2..2000000], chunkSize = 20000
        //         => [2..20001], [20002..40001], ... itd.
        //       Ostatni chunk może być krótszy.
        //
        //    2. Dla każdego chunku utwórz osobne zadanie Task<int>, które:
        //         - policzy, ile jest liczb pierwszych we własnym podzakresie,
        //         - regularnie sprawdza 'cancellationToken':
        //               cancellationToken.ThrowIfCancellationRequested();
        //           (np. na początku pętli, albo co kilka iteracji)
        //
        //    3. Użyj 'Task.Run' do uruchomienia obliczeń dla każdego chunku.
        //       W środku lambdy użyj metody pomocniczej 'IsPrime(n)'.
        //
        //    4. Zbierz wszystkie taski w kolekcji (np. List<Task<int>>),
        //       a następnie:
        //           var partialCounts = await Task.WhenAll(listaTasków);
        //
        //    5. Zsumuj wszystkie elementy 'partialCounts' i zwróć jako wynik.
        //
        //    6. Jeśli 'cancellationToken' zostanie anulowany w trakcie:
        //       - któryś z tasków ma rzucić OperationCanceledException
        //         (np. przez ThrowIfCancellationRequested),
        //       - cała metoda powinna zakończyć się tym wyjątkiem
        //         (Task.WhenAll przepuści go dalej).
        //
        //  Podpowiedzi techniczne:
        //    - Dodaj tu 'async' i zwracaj Task<int>:
        //         public static async Task<int> CountPrimesParallelAsync(...)
        //    - W pętli po chunkach pamiętaj, żeby NIE używać bezpośrednio
        //      zmiennych pętli w lambdzie, tylko skopiować je do zmiennych
        //      lokalnych, np.:
        //           int start = ...;
        //           int end = ...;
        //           int localStart = start;
        //           int localEnd = end;
        //      inaczej lambdy złapią referencję do tej samej zmiennej.
        //
        //    - W środku Task.Run:
        //         * tworzysz lokalny 'int localCount = 0;'
        //         * pętla for po n od localStart do localEnd
        //             - cancellationToken.ThrowIfCancellationRequested();
        //             - jeśli IsPrime(n) => localCount++
        //         * na końcu 'return localCount;'
        //
        //    - Po Task.WhenAll:
        //         * dostajesz int[] partialCounts
        //         * zrób zwykłą pętlę albo LINQ do zsumowania.
        public async static Task<int> CountPrimesParallelAsync(
            int fromInclusive,
            int toInclusive,
            int chunkSize,
            CancellationToken cancellationToken)
        {   
            int workers = Environment.ProcessorCount;
            List<Task<int>> tasks = new List<Task<int>>();
            for (int start = fromInclusive; start < workers+fromInclusive; start++)
            {
                int start_loc = start;
                
                tasks.Add(Task.Run(() =>
                {   
                    int count = 0;
                    for (int i = start_loc; i <= toInclusive ; i+=workers)
                    {   
                        // cancellationToken.ThrowIfCancellationRequested();
                        if (IsPrime(i))
                        {
                            count++;
                        }
                    }
                    return count;

                }));


            }
            
            int[] counts  = await Task.WhenAll(tasks);
            return counts.Sum();
        }
    }
}
