using System;
using System.Collections.Generic;
//using System.Collections.Immutable; // can't get this on mono, wanted it for a threadsafe recursive version.  I'm gonna kludge it below
using System.Linq;

// Aliased tuples avoid boilerplate for intermediate types.
using FragmentPair = System.ValueTuple<string, string>;
using FragmentPairIntersection = System.ValueTuple<string, string, int>;
using SelectorFunction = System.Func<System.ValueTuple<string, string>, System.ValueTuple<string, string, int>>;

namespace DocumentReconstructor
{
    /// <summary>
    /// This is the behaviour of the program.
    ///
    /// Test Driven Design requires "coding to interfaces" be strictly
    /// adhered to, so this is created first and tests are written for
    /// it in advance.
    /// </summary>
    public interface IDocumentReconstructor
    {
        string Reconstruct(List<string> fragments);
    }


    public static class StringExtensions
    {
        /// <summary>
        /// Take <paramref name="len"/> characters from the <strong>start</strong> of <paramref name="inStr"/>
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string prefix(this string inStr, int len) => inStr.Substring(0, len);


        /// <summary>
        /// Take <paramref name="len"/> characters from the <strong>end</strong> of <paramref name="inStr"/>
        /// </summary>
        /// <param name="inStr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string suffix(this string inStr, int len) => inStr.Substring(inStr.Length - len, len);

    }

    /// <summary>
    /// Implementation for <see cref="IDocumentReconstructor"/>
    ///
    /// See readme.md for motivation.
    /// </summary>
    public class DocumentConstructorImpl : IDocumentReconstructor
    {
        /// <summary>
        /// In a threaded environment, allowing this to be set arbitrarily is dangerous.
        /// </summary>
        public readonly Func<List<string>, string> implementedFunction;

        /// <summary>
        /// Build a new Document Constructor with the given strategy.
        /// </summary>
        /// <param name="selector"></param>
        public DocumentConstructorImpl(Func<List<string>, string> implementedFunction)
        {
            this.implementedFunction = implementedFunction;
        }

        /// <summary>
        /// get distinct pairs and the minimum lenght of the pairs
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        private static IEnumerable<FragmentPair> DistinctPairsOf(List<string> fragments)
        {
            foreach (string leftFrag in fragments)
            {
                foreach (string rightFrag in fragments)
                {
                    if (!rightFrag.Equals(leftFrag))
                        yield return (leftFrag, rightFrag);
                }
            }
        }

        /// <summary>
        /// Implements a greedy match.
        /// This is public so that it can be used to allow more complex matching strategies.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static FragmentPairIntersection GreedyMatchIntersection(FragmentPair pair)
        {
            (string leftFrag, string rightFrag) = pair;
            int len = Math.Min(leftFrag.Length, rightFrag.Length);
            int overlap = 0;
            for (int i = 1; i < len; i++)
            {
                if (leftFrag.suffix(i).Equals(rightFrag.prefix(i)))
                {
                    overlap = i; // just returning this would obscure smaller intersections behind larger ones
                    return (leftFrag, rightFrag, i);
                }
            }
            return (leftFrag, rightFrag, overlap);
        }

        /// <summary>
        /// Applies a selecto function to the list of fragments, producing a list of pairs with 
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="selectionFunc"></param>
        /// <returns></returns>
        private static FragmentPairIntersection BestMatchIn(IEnumerable<FragmentPair> pairs, Func<FragmentPair, FragmentPairIntersection> selectionFunc) {
            return pairs
                .Select(selectionFunc)
                .OrderBy(pair => -pair.Item3)
                .First();
        }


        /// <summary>
        /// Given a pair and the intersection length, weld them into a single string.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        private static string WeldIntersectedPair(FragmentPairIntersection pair)
        {
            (string left, string right, int intersect) = pair;
            string intersection = left.suffix(intersect); // right.prefix(pair.Item3) // equivalent
            string leftSlice = left.prefix(left.Length - intersect);
            string rightSlice = right.suffix(right.Length - intersect);
            string weld = $"{leftSlice}{intersection}{rightSlice}";
            return weld;
        }

        /// <summary>
        /// collapses the list by one
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        private static List<string> ReduceList(List<string> fragments, SelectorFunction selectionFunc)
        {
            var pair = BestMatchIn(DistinctPairsOf(fragments), selectionFunc);
            if (pair.Item3 == 0) // no matches remain
            {
                return fragments.OrderBy(f => -f.Length).Take(1).ToList();
            }
            var weldedPair = WeldIntersectedPair(pair);
            fragments.Remove(pair.Item1);
            fragments.Remove(pair.Item2);
            fragments.Add(weldedPair);
            return fragments;
        }

        /// <summary>
        /// provide an implentation that mutates a list in place by iteration.  This is generally the more efficient approach
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static Func<List<string>, string> ByMutableIterative(SelectorFunction selector)
        {
            return (List<string> fragments) =>
            {
                do
                {
                    fragments = ReduceList(fragments, selector);

                } while (fragments.Count() > 1);
                return fragments.First();
            };
        }

        /// <summary>
        /// provide an implentation that recursively reduces the fragments without modifying the list.
        /// This is not effiecient, but is generally safer in multi-threaded environemnts.
        ///
        /// This should ideally be using System.Collections.Immutable, but on mono/mac I can't import it.
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static Func<List<string>, string> ByImmutableRecursive(SelectorFunction selector)
        {
            // Can't be a lambda and be recursive, not without some silly declaration syntax
            string ReconstructRecursive (List<string> fragments)
            {
                if (fragments.Count() == 1) return fragments.First();

                return ReconstructRecursive(ReduceList(new List<string>(fragments), selector));
            };

            return ReconstructRecursive;
        }


        /// <summary>
        /// Entry point function.  delegates to the function passed to the constructor.
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public string Reconstruct(List<string> fragments) => implementedFunction(fragments);


    }
}
