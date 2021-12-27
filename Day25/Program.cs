using System;
using System.IO;

namespace Day25
{
    class Program
    {
        private static char[,] map;
        private static int mapHeight;
        private static int mapWidth;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");
            mapHeight = lines.Length;
            mapWidth = lines[0].Length;
            map = new char[mapWidth, mapHeight];
            
            for (int y = 0; y < mapHeight; y++)
            {
                lines[y] = lines[y].Replace('.', default);
                for (int x = 0; x < mapWidth; x++)
                {
                    map[x, y] = lines[y][x];
                }
            }
            PrintMap();

            int steps = 0;
            bool somethingChanged;
            do
            {
                somethingChanged = false;
                if (SimulateStep('>'))
                    somethingChanged = true;
                if (SimulateStep('v'))
                    somethingChanged = true;

                if (steps % 50 == 0)
                    PrintMap();
                steps++;
            } while (somethingChanged);
            
            PrintMap();
            Console.WriteLine();
            Console.WriteLine("Part 1: " + steps);
            
        }

        private static void PrintMap()
        {
            Console.WriteLine();
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Console.Write(map[x,y] == default ? "." : map[x,y]);
                }
                Console.WriteLine();
            }
        }

        private static bool SimulateStep(char dir)
        {
            bool somethingChanged = false;
            char[,] next = new char[mapWidth, mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (map[x, y] == default)
                        continue;
                    if (map[x, y] != dir)
                    {
                        next[x, y] = map[x, y];
                        continue;
                    }

                    var newPos = GetNewPosition(x, y);
                    next[newPos.x, newPos.y] = map[x, y];

                    if (newPos.x != x || newPos.y != y)
                        somethingChanged = true;
                }
            }
            map = next;
            return somethingChanged;
        }

        private static (int x, int y) GetNewPosition(int x, int y)
        {
            var dir = map[x, y];
            int newX = x;
            int newY = y;
            
            if (dir == '>')
                newX = x == mapWidth - 1 ? 0 : x + 1;
            if (dir == 'v')
                newY = y == mapHeight - 1 ? 0 : y + 1;
                //newY = y == 0 ? mapHeight - 1 : y - 1;
            
            if (map[newX, newY] == default)
                return (newX, newY);
            return (x, y);
        }
    }
}