using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    // Twoje własne API na progress bar
    public interface IProgressBar : IProgress<int>
    {
        // Historia wszystkich zgłoszonych wartości.
        // Użyję tego w testerze, więc musi działać :)
        IReadOnlyList<int> History { get; }

        // Ostatnia zgłoszona wartość procentowa (0..100)
        int LastValue { get; }
    }

    // TODO (Exercise7, część 1):
    //  Zaimplementuj klasę LabProgressBar, która:
    //
    //    - implementuje IProgressBar
    //    - przechowuje historię zgłoszonych wartości w liście (List<int>)
    //    - LastValue zwraca:
    //         - ostatnią zgłoszoną wartość jeśli coś było
    //         - albo 0, jeśli jeszcze nic nie zgłoszono
    //    - w Report(int value):
    //         - dodaje value do historii
    //         - możesz (opcjonalnie) narysować pasek w konsoli:
    //              [#####.....]  42%
    //           ALE to nie jest wymagane do zaliczenia
    //         - załóż, że value jest w zakresie 0..100, ale możesz dodać clamping
    //
    //  Podpowiedzi techniczne:
    //    - użyj prywatnego List<int> _history
    //    - History możesz zwrócić jako _history.AsReadOnly()
    //      albo po prostu jako IReadOnlyList<int> (List<int> już to implementuje)
    public sealed class LabProgressBar : IProgressBar
    {   
        private List<int> _history = new List<int>();
        public IReadOnlyList<int> History
        {
            get
            {
                
                return _history.AsReadOnly();
            }
        }

        public int LastValue
        {
            get
            {
                return _history.Last();
            }
        }

        public void Report(int value)
        {
            // TODO:
            //  - dodaj value do historii
            //  - (opcjonalnie) narysuj pasek w konsoli
            //  - możesz tu zrobić prosty progress bar:
            //        int width = 30;
            //        int filled = value * width / 100;
            //        ... itp.
            _history.Add(value);
            StringBuilder s = new StringBuilder(null);
            s .Append("[");
            int width = 30;
            int filled = value * width / 100;
            for (int i = 0; i < filled; i++)
            {
                s.Append("#");
            }

            for (int i = filled; i < width; i++)
            {
                s.Append('-');
            }

            s.Append(']');
            
            Console.Write($"\r{s.ToString()}");
        }
    }

    public static class Exercise7_ProgressBarClass
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Exercise7: Custom IProgressBar + class using it ===");

            var bar = new LabProgressBar();
            var cts = new CancellationTokenSource();

            // Długość "przetwarzania"
            int totalItems = 1000;

            await HeavyPipelineRunner.RunAsync(
                totalItems: totalItems,
                progress: bar,
                cancellationToken: cts.Token);

            var history = bar.History;

            bool hasValues = history.Count > 0;
            bool lastIsHundred = hasValues && history[history.Count - 1] == 100;
            bool allInRange = hasValues && history.All(v => v >= 0 && v <= 100);
            bool nonDecreasing = IsNonDecreasing(history);

            bool ok = hasValues && lastIsHundred && allInRange && nonDecreasing;

            ResultChecker.CheckCondition(
                "Exercise7 (progress bar)",
                ok,
                okMessage: "progress values non-decreasing in [0,100] and ended at 100",
                failMessage: "progress sequence incorrect (sprawdź Report i mapowanie postępu)");

            Console.WriteLine();
        }

        private static bool IsNonDecreasing(IReadOnlyList<int> values)
        {
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] < values[i - 1])
                    return false;
            }

            return true;
        }
    }

    public static class HeavyPipelineRunner
    {
        // TODO (Exercise7, część 2):
        //
        //  Zaimplementuj asynchroniczną "ciężką" operację wieloetapową,
        //  która:
        //
        //    - składa się z 3 etapów:
        //         1) "Wczytywanie" (load)
        //         2) "Przetwarzanie" (process)
        //         3) "Zapis" (save)
        //
        //    - każdy etap przechodzi po wszystkich elementach [0..totalItems-1]
        //
        //    - używa async/await:
        //         - możesz np. w każdej iteracji albo co N iteracji zrobić:
        //               await Task.Delay(1, cancellationToken);
        //           żeby nie było to mega krótkie
        //
        //    - respektuje CancellationToken:
        //         - regularnie wywołuj:
        //               cancellationToken.ThrowIfCancellationRequested();
        //           (niekoniecznie w każdej iteracji, może być co N kroków)
        //
        //    - raportuje globalny procent ukończenia (0..100) przez IProgress<int>:
        //         - licz całkowitą liczbę "kroków" jako:
        //               totalSteps = totalItems * 3   (3 etapy)
        //         - po każdym przetworzonym elemencie (w dowolnym etapie):
        //               completedSteps++
        //               int percent = completedSteps * 100 / totalSteps;
        //               progress.Report(percent);
        //
        //  Podpowiedź organizacyjna:
        //
        //    - zrób pola lokalne:
        //          int totalSteps = totalItems * 3;
        //          int completedSteps = 0;
        //
        //    - potem trzy pętle for jedna po drugiej (load, process, save),
        //      każda po totalItems elementów:
        //
        //          for (int i = 0; i < totalItems; i++)
        //          {
        //              cancellationToken.ThrowIfCancellationRequested();
        //              // symulacja pracy (Delay lub jakiś CPU-bound kawałek)
        //              completedSteps++;
        //              int percent = completedSteps * 100 / totalSteps;
        //              progress.Report(percent);
        //          }
        //
        //    - cała metoda powinna być 'async Task' i używać 'await'
        public static async Task RunAsync(
            int totalItems,
            IProgress<int> progress,
            CancellationToken cancellationToken)
        {   
            int completedItems = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int i = 0; i < totalItems; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    completedItems++;
                    progress.Report((completedItems*100)/(totalItems*3));
                }
            }
            
        }
    }
}
