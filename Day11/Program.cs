using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day11
{
    class Program
    {
        private static int _mapHeight;
        private static int _mapWidth;
        private static int[,] _map;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");
            SetInitialMap(lines);
            
            Console.WriteLine("Initial:");
            PrintMap();

            int totalFlashes = 0;
            for (int i = 0; i < 100; i++)
            {
                totalFlashes += SimulateStep();
                Console.WriteLine();
                Console.WriteLine($"Step {i+1}:");
                PrintMap();
            }
            
            Console.WriteLine("Part 1: " + totalFlashes);
            
            SetInitialMap(lines);
            int target = _mapHeight * _mapWidth;
            int step = 1;
            while (SimulateStep() < target)
                step++;
            
            Console.WriteLine();
            Console.WriteLine("Part 2: " + step);
            PrintMap();
        }

        private static void SetInitialMap(string[] lines)
        {
            _mapHeight = lines.Count();
            _mapWidth = lines[0].Length;
            _map = new int[_mapWidth, _mapHeight];
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _map[x, y] = int.Parse(lines[y][x].ToString());
                }
            }
        }

        private static int SimulateStep()
        {
            bool[,] flashed = new bool[_mapWidth, _mapHeight];
            bool somethingWasFlashed;

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    IncreaseValue(x,y);
                }
            }

            do
            {
                somethingWasFlashed = false;
                for (int y = 0; y < _mapHeight; y++)
                {
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        if (flashed[x, y])
                            continue;

                        if (_map[x, y] <= 9)
                            continue;

                        flashed[x, y] = true;
                        IncreaseValue(x - 1, y);
                        IncreaseValue(x + 1, y);
                        IncreaseValue(x, y - 1);
                        IncreaseValue(x, y + 1);
                        IncreaseValue(x + 1, y + 1);
                        IncreaseValue(x + 1, y - 1);
                        IncreaseValue(x - 1, y + 1);
                        IncreaseValue(x - 1, y - 1);
                        somethingWasFlashed = true;
                    }
                }
            } while (somethingWasFlashed);

            int numFlashes = 0;
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    if (_map[x, y] > 9)
                    {
                        _map[x, y] = 0;
                        numFlashes++;
                    }
                }
            }

            return numFlashes;
        }

        private static void IncreaseValue(int x, int y)
        {
            if (x < 0 || x >= _mapWidth)
                return;
            if (y < 0 || y >= _mapWidth)
                return;

            _map[x, y]++;
        }

        private static void PrintMap()
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int val = _map[x, y];
                    if (val == 0)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ResetColor();
                    
                    Console.Write(val);
                }
                Console.WriteLine();
            }
        }
    }
}