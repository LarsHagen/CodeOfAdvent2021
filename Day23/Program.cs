using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //new Part1.Part1().Run();
            new Part2.Part2().Run();
        }
    }

    public class Node
    {
        public readonly List<Node> Connections;
        public char EndLocation;
        public bool CanStop;
        public int Id;

        public Node(char endLocation, bool canStop)
        {
            Connections = new();
            EndLocation = endLocation;
            CanStop = canStop;
        }
    }

}