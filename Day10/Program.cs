using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");

            int syntaxErrorScore = 0;
            List<ulong> completionScores = new();
            
            foreach (var line in lines)
            {
                Stack<char> stack = new();
                var errorScoreForLine = GetSyntaxErrorForLine(line, stack);
                syntaxErrorScore += errorScoreForLine;

                if (errorScoreForLine == 0)
                {
                    completionScores.Add(GetCompletionScore(stack));
                }
            }
            
            Console.WriteLine("Part 1: " + syntaxErrorScore);
            
            completionScores.Sort();
            Console.WriteLine("Part 2: " + completionScores[completionScores.Count / 2]);
        }

        private static ulong GetCompletionScore(Stack<char> stack)
        {
            ulong score = 0;
            
            while (stack.Count > 0)
            {
                var scoreForChar = CompletionScore(stack.Pop());
                score *= 5;
                score += scoreForChar;
            }

            return score;
        }

        private static int GetSyntaxErrorForLine(string line, Stack<char> stack)
        {
            int syntaxErrorScore = 0;
            foreach (var c in line)
            {
                if (c == '(' || c == '[' || c == '{' || c == '<')
                {
                    //C is an opening character
                    stack.Push(c);
                    continue;
                }

                //C is a closing character
                var charToMatch = stack.Pop();
                if (c == ')' && charToMatch == '(')
                    continue;
                if (c == ']' && charToMatch == '[')
                    continue;
                if (c == '}' && charToMatch == '{')
                    continue;
                if (c == '>' && charToMatch == '<')
                    continue;

                //Syntax error
                syntaxErrorScore += SyntaxErrorScore(c);
                break;
            }

            return syntaxErrorScore;
        }

        public static int SyntaxErrorScore(char c)
        {
            return c switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137
            };
        }
        
        public static ulong CompletionScore(char c)
        {
            return c switch
            {
                '(' => 1,
                '[' => 2,
                '{' => 3,
                '<' => 4
            };
        }
    }
}