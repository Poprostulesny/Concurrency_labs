using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyLab
{
    public static class Exercise4_ProgressAndCancellation
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("=== Exercise4: IProgress + CancellationToken ===");

            using var progressBar = new ConsoleProgressBar();
            var recorder = new IntProgressRecorder();
            var composite = new CompositeProgress<int>(progressBar, recorder);

            var cts = new CancellationTokenSource();

            var simulator = new DownloadSimulator(composite);

            // 1. Bez anulowania – oczekujemy dojścia do 100%
            await simulator.DownloadAsync(
                totalChunks: 20,
                delayPerChunkMs: 50,
                cancellationToken: cts.Token);

            bool reachedHundred = recorder.Max >= 100;
            ResultChecker.CheckCondition(
                "Exercise4 (progress)",
                reachedHundred,
                okMessage: "progress reached 100%",
                failMessage: "progress did not reach 100% (czy wywołałeś Report(100)?)");

            // 2. Z anulowaniem – oczekujemy OperationCanceledException
            var ctsCancel = new CancellationTokenSource();
            ctsCancel.CancelAfter(100); // po chwili przerywamy

            var recorder2 = new IntProgressRecorder();
            var composite2 = new CompositeProgress<int>(progressBar, recorder2);
            simulator = new DownloadSimulator(composite2);

            bool cancelledOk = false;
            try
            {
                await simulator.DownloadAsync(
                    totalChunks: 100,
                    delayPerChunkMs: 50,
                    cancellationToken: ctsCancel.Token);
            }
            catch (OperationCanceledException)
            {
                cancelledOk = true;
            }

            ResultChecker.CheckCondition(
                "Exercise4 (cancellation)",
                cancelledOk,
                okMessage: "task observes CancellationToken",
                failMessage: "task did NOT react to CancellationToken (brak OperationCanceledException)");

            Console.WriteLine();
        }
    }

    public class DownloadSimulator
    {
        private readonly IProgress<int> _progress;

        public DownloadSimulator(IProgress<int> progress)
        {
            _progress = progress;
        }

        // TODO (Exercise4):
        //  - Zaimplementuj asynchroniczną symulację „pobierania pliku”.
        //  - Cała metoda powinna być asynchroniczna: użyj 'async' + 'await'.
        //  - Podziel pracę na 'totalChunks' kroków (np. pętla for).
        //  - W każdej iteracji:
        //      * poczekaj asynchronicznie:
        //            Task.Delay(delayPerChunkMs, cancellationToken)
        //      * oblicz procent ukończenia (0..100)
        //            int percent = ...
        //      * wywołaj: _progress.Report(percent)
        //  - Upewnij się, że anulowanie przez CancellationToken powoduje
        //    zgłoszenie OperationCanceledException:
        //      * albo przez przekazanie tokena do Task.Delay,
        //      * albo przez ręczne wywołanie:
        //            cancellationToken.ThrowIfCancellationRequested()
        //
        //  Podpowiedź:
        //    - Sygnatura docelowa:
        //        public async Task DownloadAsync(int totalChunks, int delayPerChunkMs, CancellationToken cancellationToken)
        public async Task DownloadAsync(int totalChunks, int delayPerChunkMs, CancellationToken cancellationToken)
        {
            for (int i = 0; i < totalChunks; i++)
            {   
                await Task.Delay(delayPerChunkMs, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var percent = (int)Math.Round(((i+1)/(double)totalChunks) * 100);
                _progress.Report(percent);
            }
        }
    }
}
