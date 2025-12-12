using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    public static class Exercise3_ParallelFor
    {
        public static void Run()
        {
            Console.WriteLine("=== Exercise3: Parallel.For ===");

            int[] input = Enumerable.Range(1, 5_000_000_00).ToArray();

            // SEKWENCYJNIE
            var swSequential = Stopwatch.StartNew();
            int[] expected = input.Select(x => x * x).ToArray();
            swSequential.Stop();

            // RÓWNOLEGLE (Twoja implementacja)
            var swParallel = Stopwatch.StartNew();
            int[] actual = ComputeSquaresWithParallelFor(input);
            swParallel.Stop();

            // Sprawdzenie poprawności
            ResultChecker.CheckSequence("Exercise3", expected, actual);

            // Czasy + przyspieszenie
            Console.WriteLine($"Sequential time: {swSequential.ElapsedMilliseconds} ms");
            Console.WriteLine($"Parallel   time: {swParallel.ElapsedMilliseconds} ms");

            if (swParallel.ElapsedTicks > 0)
            {
                double speedup = (double)swSequential.ElapsedTicks / swParallel.ElapsedTicks;
                Console.WriteLine($"Speedup (seq / par): {speedup:F2}x");
            }

            Console.WriteLine();
        }

        // TODO (Exercise3):
        //  - Utwórz nową tablicę 'output' o tej samej długości co 'input'.
        //  - Użyj 'Parallel.For' (System.Threading.Tasks.Parallel),
        //    aby równolegle policzyć kwadrat każdego elementu:
        //        output[i] = input[i] * input[i];
        //  - Pamiętaj, że w ciele Parallel.For każdy indeks 'i'
        //    powinien być niezależny (bez współdzielenia stanu).
        //
        //  Podpowiedź:
        //    - Sygnatura: Parallel.For(int fromInclusive, int toExclusive, Action<int> body)
        public static int[] ComputeSquaresWithParallelFor(int[] input)
        {   
            int[] output = new int[input.Length];
            Parallel.For(0, input.Length, i =>
            {
                output[i] = input[i] * input[i];
            
            });
            
            return output;
        }
    }
}
