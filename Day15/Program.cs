using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    class Program
    {
        private static Node[,] _nodes;
        private static int _mapWidth;
        private static int _mapHeight;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");
            _mapWidth = lines[0].Length;
            _mapHeight = lines.Length;
            
            _nodes = new Node[_mapWidth, _mapHeight];
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _nodes[x, y] = new Node()
                    {
                        X = x,
                        Y = y,
                        Cost = int.Parse(lines[y][x].ToString()),
                        PathCost = int.MaxValue
                    };
                }
            }

            Node start = GetNode(0, 0);
            start.PathCost = 0;
            Node end = GetNode(_mapWidth - 1, _mapHeight - 1);
            Pathfinding(start, end, new());
            
            
            Console.WriteLine("Part 1: " + end.PathCost);


            int multiplyX = 5;
            int multiplyY = 5;
            
            var newMap = new Node[_mapWidth * multiplyX, _mapHeight * multiplyY];
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {

                    for (int yy = 0; yy < multiplyY; yy++)
                    {
                        for (int xx = 0; xx < multiplyX; xx++)
                        {
                            int newX = x + (xx * _mapWidth);
                            int newY = y + (yy * _mapHeight);
                            
                            newMap[newX, newY] = new Node()
                            {
                                Cost = (GetNode(x, y).Cost + xx + yy),
                                X = newX,
                                Y = newY,
                                PathCost = int.MaxValue
                            };

                            while (newMap[newX,newY].Cost > 9)
                            {
                                newMap[newX, newY].Cost -= 9;
                            }
                        }
                    }
                }
            }
            
            _nodes = newMap;
            _mapHeight *= multiplyY;
            _mapWidth *= multiplyX;

            start = GetNode(0, 0);
            start.PathCost = 0;
            end = GetNode(_mapWidth - 1, _mapHeight - 1);
            Pathfinding(start, end, new());

            Console.WriteLine("Part 2: " + end.PathCost);
        }

        private static void Pathfinding(Node current, Node end, List<Node> toCheck)
        {
            while (current != end)
            {
                if (current == end)
                    return;

                PathfindingCheckNode(current, GetNode(current.X - 1, current.Y), end, toCheck);
                PathfindingCheckNode(current, GetNode(current.X + 1, current.Y), end, toCheck);
                PathfindingCheckNode(current, GetNode(current.X, current.Y - 1), end, toCheck);
                PathfindingCheckNode(current, GetNode(current.X, current.Y + 1), end, toCheck);

                if (toCheck.Count == 0)
                    return;

                toCheck.Sort();
                current = toCheck[0];
                toCheck.RemoveAt(0);
            }
        }

        private static void PathfindingCheckNode(Node from, Node to, Node end, List<Node> toCheck)
        {
            if (to == null)
                return;

            if (to.Previous == null || to.PathCost > from.PathCost + to.Cost)
            {
                to.Previous = from;
                to.PathCost = from.PathCost + to.Cost;
                to.PathCostWithManhattenCost = to.PathCost + to.ManhattenCost(end);

                if (toCheck.Contains(to))
                    return;
                
                toCheck.Add(to);
            }
        }

        private static Node GetNode(int x, int y)
        {
            if (x < 0 || x >= _mapWidth)
                return null;
            if (y < 0 || y >= _mapHeight)
                return null;
            
            return _nodes[x,y];
        }
         
    }
    
    public class Node : IComparable
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Cost { get; set; }
        
        public int PathCost;

        public Node Previous;

        public int PathCostWithManhattenCost;

        public int ManhattenCost(Node to)
        {
            return Math.Abs(X - to.X) + Math.Abs(Y - to.Y);
        }
        
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            if (obj is Node otherNode)
                return PathCostWithManhattenCost.CompareTo(otherNode.PathCostWithManhattenCost);
            else
                throw new ArgumentException("Object is not a Node");
        }
    }
}