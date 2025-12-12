using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    public static class Exercise2_TasksAndWhenAll
    {
        public static void Run()
        {
            Console.WriteLine("=== Exercise2: Creating tasks + Task.WhenAll ===");

            int from = 1;
            int to = 1_000;

            int expected = SequentialSumOfSquares(from, to);

            // Blokujemy się tutaj świadomie, to konsola (brak SynchronizationContext)
            int actual = SumOfSquaresWithTasksAsync(from, to)
                .GetAwaiter()
                .GetResult();

            ResultChecker.Check("Exercise2", expected, actual);
            Console.WriteLine();
        }

        private static int SequentialSumOfSquares(int fromInclusive, int toInclusive)
        {
            int sum = 0;
            for (int i = fromInclusive; i <= toInclusive; i++)
            {
                sum += i * i;
            }

            return sum;
        }

        // TODO (Exercise2):
        //  - Podziel zakres [fromInclusive, toInclusive] na kilka mniejszych przedziałów.
        //  - Dla każdego przedziału utwórz osobne zadanie (Task<int>),
        //    które policzy sumę kwadratów liczb w tym podzakresie.
        //  - Użyj 'Task.Run' do tworzenia tasków.
        //  - Uruchom wszystkie taski równolegle i poczekaj na nie za pomocą:
        //        Task.WhenAll(...)
        //  - Po otrzymaniu tablicy wyników z Task.WhenAll (int[])
        //    zsumuj je i zwróć sumę jako Task<int>.
        //
        //  Podpowiedź:
        //    - WhenAll dla kolekcji Task<int> zwraca Task<int[]>
        //    - Możesz zrobić np. listę Task<int> i potem 'await Task.WhenAll(lista)'
        public async  static Task<int> SumOfSquaresWithTasksAsync(int fromInclusive, int toInclusive)
        {
            int workers = Environment.ProcessorCount;
            var tasks = new Task<int>[workers];
            
          
            for (int i = 0; i < workers; i++)
            {
                int workerId = i;
                tasks[i] = Task.Run(() =>
                {
                    int partial_sum = 0;
                    for (int y = fromInclusive+workerId;y <= toInclusive; y+=workers)
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