using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    public class Part2
    {
        public void Run(string input, Dictionary<string, char> rules)
        { 
            var iteration = Initialize(input, rules);

            for (int i = 0; i < 40; i++)
            {
                var nextIteration = new Iteration
                {
                    IterationNumber = iteration.IterationNumber + 1
                };
                
                foreach (var pair in iteration.PairCount)
                {
                    nextIteration.Add(pair.Key.Left, pair.Value);
                    nextIteration.Add(pair.Key.Right, pair.Value);
                }

                iteration = nextIteration;
            }
            
            Dictionary<char, ulong> charCount = new();

            foreach (var amount in iteration.PairCount)
            {
                var c = amount.Key.Text[0];
                
                if (!charCount.ContainsKey(c))
                    charCount.Add(c,0);
                
                charCount[c] += amount.Value;
            }

            charCount[input.Last()] ++;

            
            var ordered = charCount.OrderBy(x => x.Value).Where(x => x.Value > 0).ToList();
            
            Console.WriteLine($"Part 2 ({iteration.IterationNumber} iterations):");
            Console.WriteLine("Low: " + ordered.First().Key + " (" + ordered.First().Value + ")");
            Console.WriteLine("High: " + ordered.Last().Key + " (" + ordered.Last().Value + ")");
            Console.WriteLine(ordered.Last().Value - ordered.First().Value);
        }

        private Iteration Initialize(string input, Dictionary<string, char> rules)
        {
            Pair.rules = rules;
            Iteration iteration = new();
            for (int i = 0; i < input.Length - 1; i++)
            {
                iteration.Add(Pair.Get(input.Substring(i, 2)), 1);
                iteration.IterationNumber = 0;
            }

            return iteration;
        }
    }

    public class Iteration
    {
        public int IterationNumber;
        private Dictionary<Pair, ulong> pairCount = new();
        public IReadOnlyDictionary<Pair, ulong> PairCount => pairCount;

        public void Add(Pair node, ulong amount)
        {
            if (!pairCount.ContainsKey(node))
                pairCount.Add(node, 0);

            pairCount[node] += amount;
        }

    }

    public class Pair
    {
        public static Dictionary<string, char> rules;
        private static Dictionary<string, Pair> allNodes = new();
        
        public string Text;
        public Pair Left;
        public Pair Right;

        public static Pair Get(string pair)
        {
            if (allNodes.ContainsKey(pair))
                return allNodes[pair];

            Pair newNode = new Pair();
            newNode.Text = pair;
            allNodes.Add(pair, newNode);

            if (rules.ContainsKey(pair))
            {
                var toInsert = rules[pair].ToString();
                newNode.Left = Get(pair[0] + toInsert);
                newNode.Right = Get(toInsert + pair[1]);
            }
            
            return newNode;
        }
    }
}