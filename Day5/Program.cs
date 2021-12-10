using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    class Program
    {   
        //one 8111
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var input = File.ReadLines("Input.txt");
            var lines = input.Select(x => new Line(x)).ToList();

            Console.WriteLine();
            Console.WriteLine("Part 1: " + GetHitsWithTwoOrMore(lines, false));
            Console.WriteLine("Part 2: " + GetHitsWithTwoOrMore(lines, true));
        }

        private static int GetHitsWithTwoOrMore(List<Line> lines, bool includeDiagonal)
        {
            Dictionary<(int x, int y), int> hits = new();

            foreach (var line in lines)
            {
                foreach ((int x, int y) coordinate in line.GetCellsInLine(includeDiagonal))
                {
                    if (!hits.ContainsKey(coordinate))
                        hits.Add(coordinate, 0);

                    hits[coordinate]++;
                }
            }

            /*Console.WriteLine();
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (hits.ContainsKey((x,y)))
                        Console.Write(hits[(x,y)]);
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
                
            }*/
            
            int hitsWithTwoOrMore = hits.Values.Count(x => x >= 2);
            return hitsWithTwoOrMore;
        }
    }

    internal class Line
    {
        private readonly (int x, int y) _start;
        private readonly (int x, int y) _end;
        
        public Line(string input)
        {
            int[] split = input.Replace(" -> ", ",").Split(',').Select(int.Parse).ToArray();
            _start = (split[0], split[1]);
            _end = (split[2], split[3]);
        }

        public IList<(int x, int y)> GetCellsInLine(bool includeDiagonal)
        {
            List<(int x, int y)> result = new();
            int xDiff = _end.x - _start.x;
            int yDiff = _end.y - _start.y;

            int xDir = Math.Clamp(xDiff, -1, 1);
            int yDir = Math.Clamp(yDiff, -1, 1);

            if (!includeDiagonal && xDir != 0 && yDir != 0)
                return result;

            List<int> xValues = new();
            List<int> yValues = new();

            int steps = Math.Max(Math.Abs(yDiff), Math.Abs(xDiff));
            
            for (int i = 0; i <= steps; i++)
                yValues.Add(_start.y + yDir * i);
            for (int i = 0; i <= steps; i++)
                xValues.Add(_start.x + xDir * i);
            
            for (int i = 0; i <= steps; i++)
                result.Add((xValues[i], yValues[i]));
            
            return result;
        }
    } 
}