using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day19
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Scanner> scanners = new();
            Scanner current = null;
            foreach (var line in File.ReadAllLines("Input.txt"))
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                
                if (line.Contains("---"))
                {
                    current = new Scanner();
                    scanners.Add(current);
                }
                else
                {
                    var coordinates = line.Split(',').Select(int.Parse).ToArray();
                    current.beacons.Add(new IntVector3(
                        coordinates[0],
                        coordinates[1],
                        coordinates[2]));
                }
            }

            scanners[0].Position = new IntVector3(0, 0, 0);

            
            
            while (scanners.Any(s => s.Position == null))
            {
                foreach (var scannerA in scanners)
                {
                    if (scannerA.Position == null)
                        continue;

                    foreach (var scannerB in scanners)
                    {
                        if (scannerA == scannerB)
                            continue;

                        if (scannerB.Position != null)
                            continue;

                        if (Overlaps(scannerA, scannerB, out var offset))
                            scannerB.Position = scannerA.Position + offset;
                    }
                }
            }

            List<IntVector3> uniqueBeacons = new();
            foreach (var scanner in scanners)
            {
                scanner.Translate(scanner.Position.X, scanner.Position.Y, scanner.Position.Z);
                foreach (var beacon in scanner.beacons)
                {
                    if (uniqueBeacons.Any(x => x.Equals(beacon)))
                        continue;
                    uniqueBeacons.Add(beacon);
                }
            }
            Console.WriteLine("Part 1: " + uniqueBeacons.Count);

            int largestDistance = 0;
            foreach (var scannerA in scanners)
            {
                foreach (var scannerB in scanners)
                {
                    if (scannerA == scannerB)
                        continue;

                    largestDistance = Math.Max(
                        ManhattenDistance(scannerA.Position, scannerB.Position),
                        largestDistance);
                }
            }
            
            Console.WriteLine("Part 2: " + largestDistance);
        }

        private static int ManhattenDistance(IntVector3 a, IntVector3 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }
        
        private static bool Overlaps(Scanner scannerA, Scanner scannerB, out IntVector3 offset)
        {
            foreach (var nextRotation in scannerB.FullRotationSequence)
            {
                if (OverlapWithCurrentRotation(scannerA, scannerB, out offset)) 
                    return true;
                nextRotation();
            }
            return OverlapWithCurrentRotation(scannerA, scannerB, out offset);
        }

        private static bool OverlapWithCurrentRotation(Scanner scannerA, Scanner scannerB, out IntVector3 offset)
        {
            foreach (var beaconB in scannerB.beacons)
            {
                foreach (var beaconA in scannerA.beacons)
                {
                    var diff = beaconA - beaconB;

                    scannerB.Translate(diff.X, diff.Y, diff.Z);
                    var overlaps = scannerB.NumOverlaps(scannerA);
                    scannerB.Translate(-diff.X, -diff.Y, -diff.Z);

                    if (overlaps >= 12)
                    {
                        Console.WriteLine("Found overlap");
                        offset = diff;
                        return true;
                    }
                }
            }

            offset = null;
            return false;
        }
    }

    public class Scanner
    {
        public List<IntVector3> beacons = new();
        public IntVector3 Position;
        
        public List<Action> FullRotationSequence => new()
        {
            RotateZ,
            RotateZ,
            RotateZ,

            RotateY,
            RotateX,
            RotateX,
            RotateX,

            RotateY,
            RotateZ,
            RotateZ,
            RotateZ,

            RotateY,
            RotateX,
            RotateX,
            RotateX,

            RotateZ,
            RotateY,
            RotateY,
            RotateY,

            RotateZ, RotateZ,
            RotateY,
            RotateY,
            RotateY
        };

        public int NumOverlaps(Scanner other)
        {
            int overlaps = 0;
            foreach (var becaon in beacons)
            {
                foreach (var otherBecaon in other.beacons)
                {
                    if (becaon.Equals(otherBecaon))
                        overlaps++;
                }
            }

            return overlaps;
        }

        public void Translate(int x, int y, int z)
        {
            foreach (var becaon in beacons)
            {
                becaon.X += x;
                becaon.Y += y;
                becaon.Z += z;
            }
        }
        
        public void RotateZ()
        {
            foreach (var becaon in beacons)
            {
                int newX = becaon.Y;
                int newY = -becaon.X;
                becaon.X = newX;
                becaon.Y = newY;
            }
        }
        
        public void RotateX()
        {
            foreach (var becaon in beacons)
            {
                int newY = becaon.Z;
                int newZ = -becaon.Y;
                becaon.Y = newY;
                becaon.Z = newZ;
            }
        }
        
        public void RotateY()
        {
            foreach (var becaon in beacons)
            {
                int newX = becaon.Z;
                int newZ = -becaon.X;
                becaon.X = newX;
                becaon.Z = newZ;
            }
        }
    }

    public class IntVector3
    {
        public int X;
        public int Y;
        public int Z;

        public IntVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
            => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
            => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        
        public override bool Equals(object? obj)
        {
            if (obj is IntVector3 other)
            {
                return X == other.X &&
                       Y == other.Y &&
                       Z == other.Z;
            }

            return false;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
}