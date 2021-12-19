using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var target = new Target(155, 182, -117, -67);
            //var target = new Target(20, 30, -10, -5);

            int x = 1;
            int y = 0;

            var report = RunSimulation(x, y, target, false);
            while (!report.hitTarget)
            {
                x++;
                report = RunSimulation(x, y, target, false);
            }
            Console.WriteLine("Optimal x: " + x);

            report = RunSimulation(x, y, target, true);
            while (!report.hitTarget)
            {
                y++;
                report = RunSimulation(x, y, target, true);
            }
            Console.WriteLine("Min y: " + y);

            ShotReport bestShot = report;
            List<ShotReport> hits = new();
            for (y = 0; y < 1000; y++)
            {
                report = RunSimulation(x, y, target, true);
                if (report.hitTarget)
                {
                    hits.Add(report);
                    if (report.maxHeight > bestShot.maxHeight)
                    {
                        Console.WriteLine("Best shot: " + x + "," + y);
                        bestShot = report;
                    }
                }
            }
            
            Console.WriteLine("Part 1: " + bestShot.maxHeight);

            List<int> allPossibleXHits = new();
            int fastestX = target.maxX;
            for (x = fastestX; x > 1; x--)
            {
                if (RunSimulation(x, 0, target, false).hitTarget)
                    allPossibleXHits.Add(x);
            }

            hits = new();
            for (int yVelocity = target.minY; yVelocity <= bestShot.initialY; yVelocity++)
            {
                foreach (var xVelocity in allPossibleXHits)
                {
                    report = RunSimulation(xVelocity, yVelocity, target, true);
                    if (report.hitTarget)
                        hits.Add(report);
                }
            }
            
            Console.WriteLine("Part 2: " + hits.Count);
        }

        private static ShotReport RunSimulation(int velocityX, int velocityY, Target target, bool simulateGravity)
        {
            int initialX = velocityX;
            int initialY = velocityY;
            int x = 0;
            int y = simulateGravity ? 0 : target.minY + 1;

            int maxY = 0;
            
            while (x <= target.maxX && y >= target.minY)
            {
                maxY = Math.Max(y, maxY);
                
                x += velocityX;


                if (simulateGravity)
                {
                    y += velocityY;
                    velocityY--;
                }

                if (velocityX > 0)
                    velocityX--;

                if (target.IsPointInTarget(x, y))
                    return new(initialX, initialY, true,maxY, x, y);

                if (!simulateGravity && velocityX == 0)
                    return new(initialX, initialY, false, maxY, x, y);


            }
            
            return new(initialX, initialY, false, maxY, x, y);
        }
    }

    public class ShotReport
    {
        public int initialX;
        public int initialY;
        public bool hitTarget;
        public int maxHeight;
        public int endX;
        public int endY;

        public ShotReport(int initialX, int initialY, bool hitTarget, int maxHeight, int endX, int endY)
        {
            this.initialX = initialX;
            this.initialY = initialY;
            this.hitTarget = hitTarget;
            this.maxHeight = maxHeight;
            this.endX = endX;
            this.endY = endY;
        }
    }
    
    public class Target
    {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        public Target(int minX, int maxX, int minY, int maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        public bool IsPointInTarget(int x, int y)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }

        public void Print()
        {
            Console.WriteLine($"({minX},{maxX}) ({minY},{maxY})");
        }
    }
}