using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    class Program
    {
        private const int stringLength = 12;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IList<string> input = File.ReadLines("Input.txt").ToList();
            
            Console.WriteLine("Part 1: " + Part1(input));
            Console.WriteLine("Part 2: " + Part2(input));
        }

        private static int Part1(IList<string> input)
        {
            int[] ones = new int[stringLength];
            int[] zeroes = new int[stringLength];

            foreach (string s in input)
            {
                for (var index = 0; index < s.Length; index++)
                {
                    char letter = s[index];
                    if (letter == '0')
                        zeroes[index]++;
                    else
                        ones[index]++;
                }
            }

            string gamma = "";
            string epsilon = "";
            for (int i = 0; i < stringLength; i++)
            {
                gamma += ones[i] > zeroes[i] ? '1' : '0';
                epsilon += ones[i] > zeroes[i] ? '0' : '1';
            }
            var gammaDecimal = Convert.ToInt32(gamma, 2);
            var epsilonDecimal = Convert.ToInt32(epsilon, 2);

            return gammaDecimal * epsilonDecimal;
        }
        
        private static int Part2(IList<string> input)
        {
            string gamma = GetGammaRecursive(0, input, "");
            string epsilon = GetEpsilonRecursive(0, input, "");
            var gammaDecimal = Convert.ToInt32(gamma, 2);
            var epsilonDecimal = Convert.ToInt32(epsilon, 2);

            return gammaDecimal * epsilonDecimal;
        }

        private static string GetGammaRecursive(int i, IList<string> input, string result)
        {
            if (i >= stringLength)
                return result;
            
            if (input.Count <= 1)
                return input[0];
            
            List<string> ones = new();
            List<string> zeroes = new();
            
            foreach (string s in input)
            {
                char letter = s[i];
                if (letter == '0')
                    zeroes.Add(s);
                else
                    ones.Add(s);
            }

            i++;
            
            if (zeroes.Count > ones.Count)
                return GetGammaRecursive(i, zeroes, result + '0');
            else
                return GetGammaRecursive(i, ones, result + '1');
        }
        
        
        private static string GetEpsilonRecursive(int i, IList<string> input, string result)
        {
            if (i >= stringLength)
                return result;
                
            if (input.Count == 1)
                return input[0];
            
            List<string> ones = new();
            List<string> zeroes = new();
            
            foreach (string s in input)
            {
                char letter = s[i];
                if (letter == '0')
                    zeroes.Add(s);
                else
                    ones.Add(s);
            }

            i++;

            if (zeroes.Count <= ones.Count)
                return GetEpsilonRecursive(i, zeroes, result + '0');
            else
                return GetEpsilonRecursive(i, ones, result + '1');
        }
    }
}