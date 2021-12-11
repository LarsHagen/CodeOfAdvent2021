using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day7
{
    class Program
    {
        private static List<int> crabPositions;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            crabPositions = File.ReadAllText("Input.txt").Split(",").Select(int.Parse).ToList();

            Console.WriteLine("Part 1: " + Part1());
            Console.WriteLine("Part 2: " + Part2());
        }

        private static int Part1()
        {
            var low = crabPositions.Min();
            var high = crabPositions.Max();
            
            Dictionary<int, int> calculated = new();
            int positionToCheck = (low + high) / 2;
            int stepSize = positionToCheck;
            int fuelCost = CalculateFuelCostToPosition(positionToCheck);
            calculated.Add(positionToCheck, fuelCost);
            BinaryLikeSearch(calculated, stepSize / 2, positionToCheck, CalculateFuelCostToPosition);
            int cheapest = calculated.Last().Value;
            return cheapest;
        }
        
        private static int Part2()
        {
            var low = crabPositions.Min();
            var high = crabPositions.Max();
            
            Dictionary<int, int> calculated = new();
            int positionToCheck = (low + high) / 2;
            int stepSize = positionToCheck;
            int fuelCost = CalculateFuelCostToPositionPart2(positionToCheck);
            calculated.Add(positionToCheck, fuelCost);
            BinaryLikeSearch(calculated, stepSize / 2, positionToCheck, CalculateFuelCostToPositionPart2);
            int cheapest = calculated.Last().Value;
            return cheapest;
        }

        private static void BinaryLikeSearch(Dictionary<int, int> calculated, int stepSize, int currentPosition, Func<int, int> fuelCostCalculation)
        {
            Console.WriteLine(calculated.Last());
            var last = calculated.Last().Value;
            
            if (stepSize < 1)
                return;
            
            int left = fuelCostCalculation(currentPosition - stepSize);
            int right = fuelCostCalculation(currentPosition + stepSize);

            if (left < right && left < last)
            {
                calculated.Add(currentPosition - stepSize, left);
                BinaryLikeSearch(calculated, stepSize / 2, currentPosition - stepSize, fuelCostCalculation);
            }
            else if (right < left && right < last)
            {
                calculated.Add(currentPosition + stepSize, right);
                BinaryLikeSearch(calculated, stepSize / 2, currentPosition + stepSize, fuelCostCalculation);
            }
            else
            {
                BinaryLikeSearch(calculated, stepSize / 2, currentPosition, fuelCostCalculation);
            }
        }

        private static int CalculateFuelCostToPosition(int positionToCheck)
        {
            int fuelCost = 0;
            foreach (var crabPosition in crabPositions)
            {
                fuelCost += Math.Abs(crabPosition - positionToCheck);
            }

            return fuelCost;
        }
        
        private static int CalculateFuelCostToPositionPart2(int positionToCheck)
        {
            int fuelCost = 0;
            foreach (var crabPosition in crabPositions)
            {
                var distance = Math.Abs(crabPosition - positionToCheck);
                
                for (int i = 1; i <= distance; i++)
                    fuelCost += i;
            }

            return fuelCost;
        }
    }
}