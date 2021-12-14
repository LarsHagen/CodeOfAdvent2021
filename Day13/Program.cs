using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");

            List<FoldInstruction> foldInstructions = new();
            List<Point> points = new();
            
            foreach (var line in lines)
            {
                if (line.Contains("fold"))
                {
                    foldInstructions.Add(new FoldInstruction(line));
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                points.Add(new Point(line));
            }
            
            for (int i = 0; i < foldInstructions.Count; i++)
            {
                Fold(points, foldInstructions[i]);
                
                if (i == 0)
                    Console.WriteLine("Part 1: " + points.Distinct().Count());
            }
            
            Console.WriteLine("Part 2:");
            PrintMap(points);
        }
        
        private static void Fold(List<Point> points, FoldInstruction foldInstruction)
        {
            foreach (var point in points)
            {
                if (foldInstruction.Horizontal && point.Y > foldInstruction.Value)
                    point.Y = foldInstruction.Value - (point.Y - foldInstruction.Value);
                
                if (!foldInstruction.Horizontal && point.X > foldInstruction.Value)
                    point.X = foldInstruction.Value - (point.X - foldInstruction.Value);
            }
        }

        private static void PrintMap(List<Point> points)
        {
            int width = 0;
            int height = 0;
            foreach(var point in points)
            {
                width = Math.Max(width, point.X);
                height = Math.Max(height, point.Y);
            }

            int[,] map = new int[width + 1, height + 1];
            
            foreach (var point in points)
            {
                map[point.X, point.Y]++;
            }

            for (int y = 0; y < height + 1; y++)
            {
                for (int x = 0; x < width + 1; x++)
                {
                    Console.Write(map[x,y] == 0 ? " " : "X");
                }

                Console.WriteLine();
            }
        }
    }

    public class Point
    {
        public int X;
        public int Y;

        public Point(string input)
        {
            var parsed = input.Split(',').Select(int.Parse).ToArray();
            X = parsed[0];
            Y = parsed[1];
        }
        
        public override bool Equals(object obj)
        {
            if (obj is not Point other)
            {
                return false;
            }

            return X == other.X && Y == other.Y;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
    
    public class FoldInstruction
    {
        public readonly int Value;
        public readonly bool Horizontal;
        
        public FoldInstruction(string input)
        {
            input = input.Replace("fold along ", null);
            var split = input.Split("=");
            Value = int.Parse(split[1]);
            Horizontal = split[0] == "y";
        }
    }
}