using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day20
{
    class Program
    {
        private static HashSet<(int x, int y)> _map = new();
        private static bool[] _enhancementAlgorithm;
        private static int minX;
        private static int maxX;
        private static int minY;
        private static int maxY;
        
        
        private static readonly (int x, int y)[] Kernel = {
            (-1, -1), (0, -1), (1, -1),
            (-1, 0), (0, 0), (1, 0),
            (-1, 1), (0, 1), (1, 1),
        };


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var lines = File.ReadAllLines("Input.txt");
            
            _enhancementAlgorithm = lines[0].Select(x => x == '#').ToArray();

            maxX = lines[2].Length;
            maxY = lines.Length - 2;
            
            for (int i = 2; i < lines.Length; i++)
            {
                var y = i - 2;
                for (int x = 0; x < lines[i].Length; x++)
                {
                    if (lines[i][x] == '#')
                        _map.Add((x, y));
                }
            }
            
            Console.WriteLine();

            int iteration = 0;
            while (iteration < 50)
            {
                //Because 000000000 in algorithm is '#' and 111111111 is '.',
                //Then outside of map will switch on each iteration.
                bool outsideIsInverted = iteration % 2 != 0;
                RunEnhancement(outsideIsInverted);
                iteration++;
                
                if (iteration == 2)
                    Console.WriteLine("Part 1: " + _map.Count);
            }
            Console.WriteLine("Part 2: " + _map.Count);
        }

        private static void RunEnhancement(bool outsideIsInverted)
        {
            var newMap = new HashSet<(int,int)>();

            for (int y = minY - 1; y < maxY + 1; y++)
            {
                for (int x = minX - 1; x < maxX + 1; x++)
                {
                    var algorithmIndex = GetEnhancementIndex(x, y, outsideIsInverted);
                    if (_enhancementAlgorithm[algorithmIndex])
                        newMap.Add((x, y));
                }
            }
            
            minX -= 1;
            minY -= 1;
            maxX += 1;
            maxY += 1;

            _map = newMap;
        }

        private static void PrintMap()
        {
            Console.WriteLine($"Map ({minX},{minY}),({maxX},{maxY}):");
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    Console.Write(_map.Contains((x,y)) ? "#" : ".");
                }
                Console.WriteLine();
            }
        }

        private static int GetEnhancementIndex(int x, int y, bool outsideIsInverted)
        {
            int val = 0;
            for (int i = 0; i < 9; i++)
            {
                if (GetPixel(x + Kernel[i].x, y + Kernel[i].y, outsideIsInverted))
                {
                    val |= 1 << (8 - i);
                }
            }
            return val;
        }

        private static bool GetPixel(int x, int y, bool outsideIsInverted)
        {
            
            if (x < minX || x >= maxX || y < minY || y >= maxY)
                return outsideIsInverted;

            return _map.Contains((x,y));
        }
    }
}