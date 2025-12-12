using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcurrencyLab
{
    public sealed class IntProgressRecorder : IProgress<int>
    {
        private readonly List<int> _values = new();

        public IReadOnlyList<int> Values => _values;
        public int Max => _values.Count == 0 ? 0 : _values.Max();

        public void Report(int value)
        {
            _values.Add(value);
        }
    }

    public sealed class CompositeProgress<T> : IProgress<T>
    {
        private readonly IReadOnlyList<IProgress<T>> _progresses;

        public CompositeProgress(params IProgress<T>[] progresses)
        {
            _progresses = progresses;
        }

        public void Report(T value)
        {
            foreach (var p in _progresses)
            {
                p.Report(value);
            }
        }
    }
}