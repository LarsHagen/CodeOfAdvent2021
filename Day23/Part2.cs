using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23.Part2
{
    public class Part2
    {
        private static List<Node> nodes;
        private static int maxDepthAllowed = 40;
        private static Dictionary<char, List<Node>> caves = new();

        public void Run()
        {
            // Input:         Example:       Ids:
            // #############  #############  
            // #...........#  #...........#  0123456789(10)
            // ###B#A#B#C###  ###B#C#B#D###  (11|15|19|23)
            //   #D#C#B#A#      #D#C#B#A#    (12|16|20|24)
            //   #D#B#A#C#      #D#B#A#C#    (13|17|21|25)
            //   #D#A#D#C#      #A#D#C#A#    (14|18|22|26)
            //   #########      #########
            
            nodes = new();
            CreateMap(nodes);

            MapPositions mapPositions = SetupInput();
            //MapPositions mapPositions = SetupExample();

            MapPositions best = null;
            LeastEnergy(mapPositions, ref best, maxDepthAllowed);
            Console.WriteLine("Part 2: " + best.Cost);
        }
        
        private static MapPositions SetupInput()
        {
            char[] startingPositions = new char[27];
            startingPositions[11] = 'B';
            startingPositions[12] = 'D';
            startingPositions[13] = 'D';
            startingPositions[14] = 'D';
            startingPositions[15] = 'A';
            startingPositions[16] = 'C';
            startingPositions[17] = 'B';
            startingPositions[18] = 'A';
            startingPositions[19] = 'B';
            startingPositions[20] = 'B';
            startingPositions[21] = 'A';
            startingPositions[22] = 'D';
            startingPositions[23] = 'C';
            startingPositions[24] = 'A';
            startingPositions[25] = 'C';
            startingPositions[26] = 'C';
            return new MapPositions(startingPositions);
        }
        private static MapPositions SetupExample()
        {
            char[] startingPositions = new char[27];
            startingPositions[11] = 'B';
            startingPositions[12] = 'D';
            startingPositions[13] = 'D';
            startingPositions[14] = 'A';
            startingPositions[15] = 'C';
            startingPositions[16] = 'C';
            startingPositions[17] = 'B';
            startingPositions[18] = 'D';
            startingPositions[19] = 'B';
            startingPositions[20] = 'B';
            startingPositions[21] = 'A';
            startingPositions[22] = 'C';
            startingPositions[23] = 'D';
            startingPositions[24] = 'A';
            startingPositions[25] = 'C';
            startingPositions[26] = 'A';
            return new MapPositions(startingPositions);
        }
        
        private static void LeastEnergy(MapPositions mapPositions, ref MapPositions best, int remainingAllowedMoves)
        {
            if (MoveAlreadyDoneInPast(mapPositions))
                return;
            if (remainingAllowedMoves <= 0)
                return;
            if (best != null && mapPositions.Cost >= best.Cost)
                return;
            
            if (IsSolved(mapPositions))
            {
                Console.WriteLine("Found a solution after " + (maxDepthAllowed - remainingAllowedMoves) + " steps. Cost: " + mapPositions.Cost);
                mapPositions.PrintSteps();
                best = mapPositions;
                return;
            }

            List<MapPositions> moves = new();
            
            for (int i = 0; i < mapPositions.Layout.Length; i++)
            {
                if (mapPositions.Layout[i] == default)
                    continue;

                List<Node> legalMoves = GetLegalMoves(nodes[i], mapPositions);
                if (legalMoves == null) //No solution found
                    continue;
                
                foreach (var move in legalMoves)
                {
                    moves.Add(new MapPositions(mapPositions, i, move.Id));
                }
            }

            if (moves.Count > 0)
            {
                foreach (var move in moves)
                {
                    LeastEnergy(move, ref best, remainingAllowedMoves - 1);
                }
            }
        }

        private static bool IsSolved(MapPositions mapPositions)
        {
            for (int i = 0; i < mapPositions.Layout.Length; i++)
            {
                if (mapPositions.Layout[i] == default)
                    continue;

                if (nodes[i].EndLocation != mapPositions.Layout[i])
                    return false;
            }

            return true;
        }

        private static Node CanMoveIntoCave(Node start, MapPositions mapPositions)
        {
            var letter = mapPositions.Layout[start.Id];

            var cave = caves[letter];
            
            var nodeInfrontOfCave = cave[0];
            var letterIn1 = mapPositions.Layout[cave[1].Id];
            var letterIn2 = mapPositions.Layout[cave[2].Id];
            var letterIn3 = mapPositions.Layout[cave[3].Id];
            var letterIn4 = mapPositions.Layout[cave[4].Id];

            if (letterIn1 != default)
                return null;
            
            var dir = nodeInfrontOfCave.Id > start.Id ? 1 : -1;
            for (int i = start.Id; i != nodeInfrontOfCave.Id; i += dir)
                if (i != start.Id && mapPositions.Layout[i] != default)
                    return null; //No free path to cave

            if (letterIn1 == default && letterIn2 == default && letterIn3 == default && letterIn4 == default)
                return cave[4];
            
            if (letterIn1 == default && letterIn2 == default && letterIn3 == default && letterIn4 == letter)
                return cave[3];
            
            if (letterIn1 == default && letterIn2 == default && letterIn3 == letter && letterIn4 == letter)
                return cave[2];
            
            if (letterIn1 == default && letterIn2 == letter && letterIn3 == letter && letterIn4 == letter)
                return cave[1];

            return null;
        }

        private static bool InCorrectSpotInCorrectCave(Node start, MapPositions mapPositions)
        {
            var letter = mapPositions.Layout[start.Id];
            var cave = caves[letter];
            
            if (start == cave[4])
                return true;

            if (cave[4].EndLocation != mapPositions.Layout[cave[4].Id])
                return false;

            if (start == cave[3])
                return true;

            if (cave[3].EndLocation != mapPositions.Layout[cave[3].Id])
                return false;

            if (start == cave[2])
                return true;

            if (cave[2].EndLocation != mapPositions.Layout[cave[2].Id])
                return false;

            if (start == cave[1])
                return true;

            return false;
        }

        private static Node CanMoveOutOfCave(Node current, MapPositions mapPositions)
        {
            var cave = caves[current.EndLocation];

            var nodeInfrontOfCave = cave[0];
            if (current == cave[1])
                return nodeInfrontOfCave;

            if (mapPositions.Layout[cave[1].Id] != default)
                return null;

            if (current == cave[2])
                return nodeInfrontOfCave;

            if (mapPositions.Layout[cave[2].Id] != default)
                return null;

            if (current == cave[3])
                return nodeInfrontOfCave;

            if (mapPositions.Layout[cave[3].Id] != default)
                return null;

            if (current == cave[4])
                return nodeInfrontOfCave;

            return null;
        }
        
        private static List<Node> GetLegalMoves(Node node, MapPositions mapPositions)
        {
            //Check if cave if filled correctly
            if (InCorrectSpotInCorrectCave(node, mapPositions))
                return null;

            bool inHallway = node.EndLocation == default;
            List<Node> allowedMoves = new List<Node>();

            if (inHallway)
            {
                var caveNode = CanMoveIntoCave(node, mapPositions);
                if (caveNode != null)
                {
                    allowedMoves.Add(caveNode);
                }
            }
            else
            {
                var hallwayNode = CanMoveOutOfCave(node, mapPositions);
                if (hallwayNode != null)
                {
                    for (int i = hallwayNode.Id - 1; i >= 0; i--)
                    {
                        if (mapPositions.Layout[i] != default) //Blocked, we can not move further in this direction
                            break;
                        if (nodes[i].CanStop)
                            allowedMoves.Add(nodes[i]);
                    }
                    for (int i = hallwayNode.Id + 1; i <= 10; i++)
                    {
                        if (mapPositions.Layout[i] != default) //Blocked, we can not move further in this direction
                            break;
                        if (nodes[i].CanStop)
                            allowedMoves.Add(nodes[i]);
                    }
                }
            }

            return allowedMoves;

        }

        private static bool MoveAlreadyDoneInPast(MapPositions input)
        {
            MapPositions previous = input.Previous;
            while (previous != null)
            {
                if (input.LayoutIsEqual(previous))
                    return true;

                previous = previous.Previous;
            }

            return false;
        }
        
        private static void CreateMap(List<Node> nodes)
        {
            //Create hallway
            for (int i = 0; i <= 10; i++)
            {
                bool canStop = i != 2 && i != 4 && i != 6 && i != 8;
                nodes.Add(new Node(default, canStop));
            }

            //Set connections for hallway
            for (int i = 0; i < 10; i++)
            {
                if (i > 0)
                    nodes[i].Connections.Add(nodes[i - 1]);
                if (i < 10)
                    nodes[i].Connections.Add(nodes[i + 1]);
            }

            //Create caves
            CreateCave('A', nodes, 2);
            CreateCave('B', nodes, 4);
            CreateCave('C', nodes, 6);
            CreateCave('D', nodes, 8);

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Id = i;
        }

        private static void CreateCave(char endLocation, List<Node> nodes, int connectionToHallway)
        {
            Node cave1 = new Node(endLocation, true);
            Node cave2 = new Node(endLocation, true);
            Node cave3 = new Node(endLocation, true);
            Node cave4 = new Node(endLocation, true);
            
            cave1.Connections.Add(nodes[connectionToHallway]);
            cave1.Connections.Add(cave2);
            cave2.Connections.Add(cave1);
            cave2.Connections.Add(cave3);
            cave3.Connections.Add(cave2);
            cave3.Connections.Add(cave4);
            cave4.Connections.Add(cave3);
            nodes.Add(cave1);
            nodes.Add(cave2);
            nodes.Add(cave3);
            nodes.Add(cave4);
            
            nodes[connectionToHallway].Connections.Add(cave1);

            caves.Add(endLocation, new List<Node> {nodes[connectionToHallway], cave1, cave2, cave3, cave4});
        }
    }
    
    
    public class MapPositions
    {
        public readonly MapPositions Previous;
        public readonly char[] Layout;
        public readonly int Cost;

        public MapPositions(char[] layout)
        {
            Previous = null;
            Layout = layout;
            Cost = 0;
        }
        
        public MapPositions(MapPositions previous, int moveFrom, int moveTo)
        {
            Previous = previous;
            Layout = new char[previous.Layout.Length];
            previous.Layout.CopyTo(Layout, 0);
            var toMove = Layout[moveFrom];
            Layout[moveFrom] = default;
            Layout[moveTo] = toMove;
            Cost = previous.Cost + GetCost(toMove) * DistBetweenNodes(moveFrom, moveTo);
        }

        public void Print()
        {
            Console.WriteLine(Cost);
            Console.WriteLine("#############");
            Console.Write("#");
            for (int i = 0; i <= 10; i++)
                Console.Write(GetPrintLetter(i));
            Console.WriteLine("#");
            Console.WriteLine($"###{GetPrintLetter(11)}#{GetPrintLetter(15)}#{GetPrintLetter(19)}#{GetPrintLetter(23)}###");
            Console.WriteLine($"  #{GetPrintLetter(12)}#{GetPrintLetter(16)}#{GetPrintLetter(20)}#{GetPrintLetter(24)}#  ");
            Console.WriteLine($"  #{GetPrintLetter(13)}#{GetPrintLetter(17)}#{GetPrintLetter(21)}#{GetPrintLetter(25)}#  ");
            Console.WriteLine($"  #{GetPrintLetter(14)}#{GetPrintLetter(18)}#{GetPrintLetter(22)}#{GetPrintLetter(26)}#  ");
            Console.WriteLine("  #########  ");
        }

        private char GetPrintLetter(int i)
        {
            return Layout[i] == default ? '.' : Layout[i];
        }

        public void PrintSteps()
        {
            Stack<Action> printActions = new();
            var map = this;
            while (map != null)
            {
                printActions.Push(map.Print);
                map = map.Previous;
            }

            int step = 0;
            while (printActions.Count > 0)
            {
                Console.WriteLine("Step: " + step);
                printActions.Pop().Invoke();
                Console.WriteLine();
                step++;
            }
        }
        
        private static int GetCost(char c)
        {
            return c switch
            {
                'A' => 1,
                'B' => 10,
                'C' => 100,
                'D' => 1000,
                _ => throw new Exception("This should not happen!")
            };
        }

        public bool LayoutIsEqual(MapPositions other)
        {
            for (int i = 0; i < Layout.Length; i++)
            {
                if (other.Layout[i] != Layout[i])
                    return false;
            }

            return true;
        }

        private int DistBetweenNodes(int a, int b)
        {
            var caveNode = a > 10 ? a : b;
            var hallwayNode = caveNode == a ? b : a;

            int infrontOfCave;
            int caveCost = 0;

            if (caveNode >= 23)
            {
                infrontOfCave = 8;
                caveCost = caveNode - 23;
            }
            else if (caveNode >= 19)
            {
                infrontOfCave = 6;
                caveCost = caveNode - 19;
            }
            else if (caveNode >= 15)
            {
                infrontOfCave = 4;
                caveCost = caveNode - 15;
            }
            else
            {
                infrontOfCave = 2;
                caveCost = caveNode - 11;
            }

            return Math.Abs(hallwayNode - infrontOfCave) + caveCost + 1;
        }
    }
}