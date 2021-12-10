using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<int> input = File.ReadLines("Input.txt").Select(int.Parse).ToList();

            Console.WriteLine("Part 1: " + Part1(input));
            Console.WriteLine("Part 2: " + Part2(input));

            List<int> sums = new();
            
        }

        private static int Part2(IList<int> input)
        {
            List<int> windowValues = new();
            for (int i = 0; i < input.Count - 2; i++)
            {
                windowValues.Add(input[i] + input[i+1] + input[i+2]);
            }
            return Part1(windowValues);
        }

        private static int Part1(IList<int> input)
        {
            int lastReading = input[0];
            int count = 0;

            foreach (var x in input)
            {
                int value = x;
                if (value > lastReading)
                {
                    count++;
                }

                lastReading = value;
            }

            return count;
        }
    }
}