using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22
{
    class Program
    {
        private static HashSet<(int x, int y, int z)> _cubes = new();
        private static List<Instruction> _instructions;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");
            _instructions = new();
            foreach (var line in lines)
            {
                _instructions.Add(new Instruction(line));
            }

            Console.WriteLine("Part 1: " + Part1(-50, 50, -50, 50, -50, 50));
            new Part2().Run(_instructions);
        }

        private static int Part1(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            _cubes.Clear();
            int on = 0;

            for (int i = _instructions.Count - 1; i >= 0; i--)
            {
                var instruction = _instructions[i];
                
                if (instruction.minX > maxX || instruction.maxX < minX)
                    continue;
                if (instruction.minY > maxY || instruction.maxY < minY)
                    continue;
                if (instruction.minZ > maxZ || instruction.maxZ < minZ)
                    continue;


                for (int x = Math.Max(instruction.minX, minX); x <= Math.Min(instruction.maxX, maxX); x++)
                {
                    for (int y = Math.Max(instruction.minY, minY); y <= Math.Min(instruction.maxY, maxY); y++)
                    {
                        for (int z = Math.Max(instruction.minZ, minZ); z <= Math.Min(instruction.maxZ, maxZ); z++)
                        {
                            if (x < minX || x > maxX)
                                continue;
                            if (y < minY || y > maxY)
                                continue;
                            if (y < minZ || y > maxZ)
                                continue;
                            
                            if (_cubes.Contains((x,y,z)))
                                continue;
                            
                            _cubes.Add((x,y,z));
                            if (instruction.on)
                                on++;
                        }
                    }
                }
            }
            
            return on;
        }

    }

    public struct Instruction
    {
        public bool on;
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
        public int minZ;
        public int maxZ;

        public Instruction(string input)
        {
            var firstSplit = input.Split(' ');
            on = firstSplit[0] == "on";
            var coords = firstSplit[1].Split(',');

            var xVals = coords[0].Replace("x=", null).Split("..").Select(int.Parse).ToArray();
            var yVals = coords[1].Replace("y=", null).Split("..").Select(int.Parse).ToArray();
            var zVals = coords[2].Replace("z=", null).Split("..").Select(int.Parse).ToArray();

            minX = xVals[0];
            maxX = xVals[1];
            minY = yVals[0];
            maxY = yVals[1];
            minZ = zVals[0];
            maxZ = zVals[1];
        }
    }
}