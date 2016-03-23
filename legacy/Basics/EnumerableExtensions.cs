using System;
using System.Collections.Generic;
using System.Linq;

namespace Basics
{
    public static class EnumerableExtensions
    {
        public static IReadOnlyList<bool> Any<T>(this IEnumerable<T> sequence, Func<T, bool> predicate1,
            Func<T, bool> predicate2) => Any(sequence, new[] { predicate1, predicate2 });

        public static IReadOnlyList<bool> Any<T>(this IEnumerable<T> sequence, IEnumerable<Func<T, bool>> predicates)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            List<MultiPredicateChecker<T>> checkers = predicates.Select(p => new MultiPredicateChecker<T>(p)).ToList();
            int matchCount = 0;
            foreach (T item in sequence)
            {
                foreach (MultiPredicateChecker<T> checker in checkers)
                {
                    if (!checker.Satisfied)
                    {
                        checker.Satisfied = checker.Predicate(item);
                        if (checker.Satisfied)
                            matchCount++;
                        if (matchCount == checkers.Count)
                            break;
                    }
                }
                if (matchCount == checkers.Count)
                    break;
            }
            return checkers.Select(c => c.Satisfied).ToList();
        }
    }

    internal sealed class MultiPredicateChecker<T>
    {
        internal MultiPredicateChecker(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        internal Func<T, bool> Predicate { get; }
        internal bool Satisfied { get; set; }
    }
}