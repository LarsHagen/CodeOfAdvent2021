using System;
using System.Collections.Generic;

namespace Day22
{
    public class Part2
    {
        public void Run(List<Instruction> instructions)
        {
            List<Cube> cubes = new();

            foreach (var instruction in instructions)
            {
                foreach (var cube in cubes.ToArray())
                {
                    var intersectionCube = cube.Intersect(instruction);
                    if (intersectionCube != null)
                    {
                        cubes.Add(intersectionCube);
                    }
                }

                if (instruction.on)
                    cubes.Add(new Cube(instruction));
            }
            
            ulong count = 0;
            foreach (var cube in cubes)
            {
                var area = cube.GetArea();
                if (cube.positive)
                    count += area;
                else
                    count -= area;
            }
            
            Console.WriteLine("Part 2: " + count);
        }

        public class Cube
        {
            public bool positive;
            
            public int minX;
            public int maxX;
            public int minY;
            public int maxY;
            public int minZ;
            public int maxZ;

            public Cube(Instruction instruction)
            {
                positive = instruction.@on;
                minX = instruction.minX;
                maxX = instruction.maxX;
                minY = instruction.minY;
                maxY = instruction.maxY;
                minZ = instruction.minZ;
                maxZ = instruction.maxZ;
            }
            public Cube(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, bool positive)
            {
                this.positive = positive;
                this.minX = minX;
                this.maxX = maxX;
                this.minY = minY;
                this.maxY = maxY;
                this.minZ = minZ;
                this.maxZ = maxZ;
            }

            public ulong GetArea()
            {
                ulong sizeX = (ulong)Math.Abs(maxX - minX) + 1;
                ulong sizeY = (ulong)Math.Abs(maxY - minY) + 1;
                ulong sizeZ = (ulong)Math.Abs(maxZ - minZ) + 1;

                return sizeX * sizeY * sizeZ;
            }
            
            public Cube Intersect(Instruction instruction)
            {
                if (instruction.maxX < minX || 
                    instruction.minX > maxX || 
                    instruction.maxY < minY || 
                    instruction.minY > maxY || 
                    instruction.maxZ < minZ || 
                    instruction.minZ > maxZ)
                    return null; //No intersection.
                    
                var intersection = new Cube(
                    Math.Max(minX, instruction.minX),
                    Math.Min(maxX, instruction.maxX),
                    Math.Max(minY, instruction.minY),
                    Math.Min(maxY, instruction.maxY),
                    Math.Max(minZ, instruction.minZ),
                    Math.Min(maxZ, instruction.maxZ),
                    !positive
                );

                return intersection;
            }
        }
    }
}