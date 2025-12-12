using System;

namespace ConcurrencyLab
{
    public sealed class ConsoleProgressBar : IProgress<int>, IDisposable
    {
        private readonly object _lock = new object();
        private bool _disposed;

        public void Report(int value)
        {
            if (_disposed) return;

            if (value < 0) value = 0;
            if (value > 100) value = 100;

            lock (_lock)
            {
                DrawBar(value);
            }
        }

        private static void DrawBar(int value)
        {
            const int barWidth = 30;
            int filled = value * barWidth / 100;

            int top = Console.CursorTop;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, top);
            Console.Write("[");
            Console.Write(new string('#', filled).PadRight(barWidth));
            Console.Write($"] {value,3}%");
            Console.CursorVisible = true;
            Console.WriteLine();
        }

        public void Dispose()
        {
            _disposed = true;
            Console.WriteLine();
        }
    }
}