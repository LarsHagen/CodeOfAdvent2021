using System;
using System.Collections.Generic;
using System.IO;

namespace Day9
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
            _mapHeight = lines.Length;
            _mapWidth = lines[0].Length;
            _map = new int[_mapWidth, _mapHeight];

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _map[x, y] = int.Parse(lines[y][x].ToString());
                }
            }

            Console.WriteLine("Part 1: " + Part1());

            Console.WriteLine("Part 2: " + Part2());
        }

        private static int Part1()
        {
            int result = 0;
            var lowSpots = FindLowSpots();
            foreach (var lowSpot in lowSpots)
            {
                result += lowSpot.value + 1;
            }

            return result;
        }

        private static int Part2()
        {
            bool[,] burnMap = new bool[_mapWidth, _mapHeight];
            List<int> basinSizes = new List<int>();
            foreach (var lowSpot in FindLowSpots())
            {
                int numBurned = 0;
                GrassFire(ref burnMap, lowSpot.x, lowSpot.y, ref numBurned);
                basinSizes.Add(numBurned);
            }
            basinSizes.Sort();
            
            return basinSizes[^1] * basinSizes[^2] * basinSizes[^3];
        }

        private static void GrassFire(ref bool[,] burnMap, int x, int y, ref int burned)
        {
            if (burnMap[x, y])
                return;
            
            burnMap[x, y] = true;
            burned++;

            if (GetHeight(x - 1, y) < 9)
                GrassFire(ref burnMap, x - 1, y, ref burned);
            if (GetHeight(x + 1, y) < 9)
                GrassFire(ref burnMap, x + 1, y, ref burned);
            if (GetHeight(x, y - 1) < 9)
                GrassFire(ref burnMap, x, y - 1, ref burned);
            if (GetHeight(x, y + 1) < 9)
                GrassFire(ref burnMap, x, y + 1, ref burned);
        }

        private static List<(int x, int y, int value)> FindLowSpots()
        {
            List<(int x, int y, int value)> lowSpots = new();
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    var value = GetHeight(x, y);

                    if (GetHeight(x, y - 1) <= value)
                        continue;
                    if (GetHeight(x, y + 1) <= value)
                        continue;
                    if (GetHeight(x - 1, y) <= value)
                        continue;
                    if (GetHeight(x + 1, y) <= value)
                        continue;

                    lowSpots.Add((x, y, value));
                }
            }

            return lowSpots;
        }

        private static int GetHeight(int x, int y)
        {
            if (x < 0)
                return int.MaxValue;
            if (x >= _mapWidth)
                return int.MaxValue;
            if (y < 0)
                return int.MaxValue;
            if (y >= _mapHeight)
                return int.MaxValue;

            return _map[x, y];
        }
    }
}