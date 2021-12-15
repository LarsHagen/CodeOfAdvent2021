using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");

            Dictionary<string, char> replacementRules = new();
            foreach (var line in lines)
            {
                if (!line.Contains("->"))
                    continue;
                var split = line.Split(" -> ");
                replacementRules.Add(split[0], split[1][0]);
            }
            
            Part1(lines, replacementRules);

            Part2 part2 = new Part2();
            part2.Run(lines[0], replacementRules);
        
        }

        private static void Part1(string[] lines, Dictionary<string, char> replacementRules)
        {
            Node start = null;
            Node current = null;
            foreach (var c in lines[0])
            {
                if (start == null)
                {
                    start = new Node(c);
                    current = start;
                }
                else
                {
                    current.InsertAfter(c);
                    current = current.Next;
                }
            }

            PrintFromNode(start);

            for (int i = 0; i < 10; i++)
            {
                RunIteration(start, replacementRules);
                PrintFromNode(start);
                Console.WriteLine();
            }

            var lowestHighest = GetMostCommonAndLeastCommon(start);
            Console.WriteLine("Part 1: " + (lowestHighest.highest - lowestHighest.lowest));
        }

        private static (int lowest, int highest) GetMostCommonAndLeastCommon(Node node)
        {
            Dictionary<char, int> amounts = new();
            CountFromNode(node, amounts);

            List<KeyValuePair<char, int>> ordered = amounts.OrderBy(x => x.Value).ToList();
            
            Console.WriteLine("Low: " + ordered.First().Key + "(" + ordered.First().Value + ")");
            Console.WriteLine("High: " + ordered.Last().Key + "(" + ordered.Last().Value + ")");
            return (ordered.First().Value, ordered.Last().Value);
        }

        private static void CountFromNode(Node current, Dictionary<char, int> amounts)
        {
            if (!amounts.ContainsKey(current.C))
                amounts.Add(current.C, 0);

            amounts[current.C]++;
            
            if (current.Next != null)
                CountFromNode(current.Next, amounts);
        }

        private static void PrintFromNode(Node node)
        {
            Console.Write(node.C);
            if (node.Next != null)
                PrintFromNode(node.Next);
            else
                Console.WriteLine();
        }

        private static void RunIteration(Node start, Dictionary<string, char> rules)
        {
            Node current = start;

            while (current != null && current.Next != null)
            {
                string pair = current.C + current.Next.C.ToString();
                current = current.Next;
                current.InsertBefore(rules[pair]);
            }
        }
    }
    
    public class Node
    {
        public Node Previous;
        public Node Next;
        public char C;

        public Node(char c)
        {
            C = c;
        }

        public void InsertAfter(char c)
        {
            var _ = Next;
            Next = new Node(c);
            Next.Previous = this;

            if (_ != null)
            {
                _.Previous = Next;
                Next.Next = _;
            }
        }
        
        public void InsertBefore(char c)
        {
            var _ = Previous;
            Previous = new Node(c);
            Previous.Next = this;

            if (_ != null)
            {
                _.Next = Previous;
                Previous.Previous = _;
            }
        }
    }
}