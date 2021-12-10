using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            IEnumerable<string> input = File.ReadLines("Input.txt");
            
            Console.WriteLine("Part 1: " + Part1(input));
            Console.WriteLine("Part 2: " + Part2(input));
        }

        private static int Part1(IEnumerable<string> input)
        {
            int horizontal = 0;
            int depth = 0;

            foreach (string s in input)
            {
                var parsed = Parse(s);
                switch (parsed.action)
                {
                    case "forward":
                        horizontal += parsed.value;
                        break;
                    case "up":
                        depth -= parsed.value;
                        break;
                    case "down":
                        depth += parsed.value;
                        break;
                }
            }

            return horizontal * depth;
        }
        
        private static int Part2(IEnumerable<string> input)
        {
            int horizontal = 0;
            int depth = 0;
            int aim = 0;

            foreach (string s in input)
            {
                var parsed = Parse(s);
                switch (parsed.action)
                {
                    case "forward":
                        horizontal += parsed.value;
                        depth += aim * parsed.value;
                        break;
                    case "up":
                        aim -= parsed.value;
                        break;
                    case "down":
                        aim += parsed.value;
                        break;
                }
            }

            return horizontal * depth;
        }

        private static (string action, int value) Parse(string s)
        {
            var split = s.Split(' ');
            return (split[0], int.Parse(split[1]));
        }
    }
}