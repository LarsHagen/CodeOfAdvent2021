using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var fish = File.ReadAllText("Input.txt").Split(",").Select(int.Parse).ToList();

            List<UInt64> fishByDays = new();
            for (int i = 0; i < 9; i++)
                fishByDays.Add((UInt64)fish.Count(x => x == (i-1)));

            Console.WriteLine("Part 1: " + FishAfterDays(fishByDays.ToList(), 80));
            Console.WriteLine("Part 2: " + FishAfterDays(fishByDays.ToList(), 256));
        }

        private static UInt64 FishAfterDays(List<UInt64> fishByDays, int days)
        {
            for (int i = 0; i <= days; i++)
                SimulateDay(ref fishByDays);

            UInt64 sum = 0;
            foreach (var numFish in fishByDays)
            {
                sum += numFish;
            }

            return sum;
        }

        private static void SimulateDay(ref List<UInt64> fishByDays)
        {
            UInt64 fishWithZero = fishByDays[0];
            
            for (int i = 0; i < fishByDays.Count - 1; i++)
                fishByDays[i] = fishByDays[i + 1];
            
            fishByDays[8] = fishWithZero;
            fishByDays[6] += fishWithZero;
        }
    }
}