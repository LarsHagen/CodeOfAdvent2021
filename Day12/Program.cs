using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        private static readonly List<Cave> Caves = new();
        private static readonly List<Connection> Connections = new();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");

            foreach (var line in lines)
            {
                var caveIDs = line.Split('-');
                Cave a = GetOrCreate(caveIDs[0]);
                Cave b = GetOrCreate(caveIDs[1]);
                Connections.Add(new Connection()
                {
                    a = a,
                    b = b
                });
            }

            var start = GetOrCreate("start");

            List<Node> pathsToEnd = new();
            PathFinding1(new Node()
            {
                cave = start,
                previous = null
            }, pathsToEnd);
            Console.WriteLine("Part 1: " + pathsToEnd.Count);
            
            pathsToEnd = new();
            PathFinding2(new Node()
            {
                cave = start,
                previous = null
            }, pathsToEnd);
            Console.WriteLine("Part 2: " + pathsToEnd.Count);
        }

        private static void PrintPath(Node node)
        {
            string output = "";
            while (node != null)
            {
                output = node.cave.id + "-" + output;
                node = node.previous;
            }
            Console.WriteLine(output);
        }

        private static void PathFinding1(Node current, List<Node> endHits)
        {
            if (current.cave.id == "end")
            {
                endHits.Add(current);
                return;
            }

            var connections = GetConnections(current.cave);
            foreach (var connection in connections)
            {
                var otherCave = connection.a == current.cave ? connection.b : connection.a;
                
                if (otherCave.id == "start")
                    continue;

                if (otherCave.isSmall && HasAlreadyVisitedCave(current, otherCave))
                    continue; //Path ends here

                PathFinding1(new Node()
                {
                    cave = otherCave,
                    previous =  current
                }, endHits);
            }
        }

        private static void PathFinding2(Node current, List<Node> endHits)
        {
            if (current.cave.id == "end")
            {
                endHits.Add(current);
                return;
            }

            var connections = GetConnections(current.cave);
            foreach (var connection in connections)
            {
                var otherCave = connection.a == current.cave ? connection.b : connection.a;

                if (otherCave.id == "start")
                    continue;
                
                if (otherCave.isSmall && HasAlreadyVisitedCave(current, otherCave) && HasVisitedAnySmallCaveTwice(current))
                    continue; //Path ends here
                
                PathFinding2(new Node()
                {
                    cave = otherCave,
                    previous =  current
                }, endHits);
            }
        }

        private static bool HasAlreadyVisitedCave(Node node, Cave cave)
        {
            while (node != null)
            {
                if (node.cave == cave)
                    return true;
                node = node.previous;
            }

            return false;
        }
        
        private static bool HasVisitedAnySmallCaveTwice(Node node)
        {
            Dictionary<Cave, int> visits = new();
            while (node != null)
            {
                if (node.cave.isSmall)
                {
                    if (!visits.ContainsKey(node.cave))
                        visits.Add(node.cave, 0);
                    visits[node.cave]++;
                }

                node = node.previous;
            }

            return visits.Values.Any(x => x > 1);
        }


        private static Cave GetOrCreate(string id)
        {
            Cave cave = Caves.FirstOrDefault(c => c.id == id);

            if (cave == null)
            {
                cave = new Cave(id);
                Caves.Add(cave);
            }

            return cave;
        }

        private static List<Connection> GetConnections(Cave from)
        {
            return Connections.Where(x => x.a == from || x.b == from).ToList();
        }
    }

    public class Node
    {
        public Cave cave;
        public Node previous;
    }
    
    public class Connection
    {
        public Cave a;
        public Cave b;
    }

    public class Cave
    {
        public string id;
        public bool isSmall;

        public Cave(string id)
        {
            this.id = id;
            isSmall = Char.IsLower(id[0]);
        }
    }
}