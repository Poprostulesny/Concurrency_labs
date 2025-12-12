using System;
using System.Collections.Generic;

namespace ConcurrencyLab
{
    public static class ResultChecker
    {
        public static void Check<T>(string exerciseName, T expected, T actual)
        {
            bool ok = EqualityComparer<T>.Default.Equals(expected, actual);
            Console.WriteLine(ok
                ? $"{exerciseName}: OK ✅"
                : $"{exerciseName}: FAIL ❌  Expected: {expected}, Actual: {actual}");
        }

        public static void CheckSequence<T>(string exerciseName, IReadOnlyList<T> expected, IReadOnlyList<T> actual)
        {
            if (expected.Count != actual.Count)
            {
                Console.WriteLine($"{exerciseName}: FAIL ❌  Length mismatch: expected {expected.Count}, got {actual.Count}");
                return;
            }

            for (int i = 0; i < expected.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(expected[i], actual[i]))
                {
                    Console.WriteLine($"{exerciseName}: FAIL ❌  Difference at index {i}: expected {expected[i]}, got {actual[i]}");
                    return;
                }
            }

            Console.WriteLine($"{exerciseName}: OK ✅");
        }

        public static void CheckCondition(string exerciseName, bool condition, string okMessage = "OK", string failMessage = "FAIL")
        {
            Console.WriteLine(condition
                ? $"{exerciseName}: {okMessage} ✅"
                : $"{exerciseName}: {failMessage} ❌");
        }
    }
}