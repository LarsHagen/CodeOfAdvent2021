using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23.Part1
{
    public class Part1
    {
        private static List<Node> nodes;
        private static int maxDepthAllowed = 20;
        
        public void Run()
        {
            // Input:         Example:       Ids:
            // #############  #############  
            // #...........#  #...........#  0123456789(10)
            // ###B#A#B#C###  ###B#C#B#D###  (11|13|15|17)
            //   #D#A#D#C#      #A#D#C#A#    (12|14|16|18)
            //   #########      #########
            
            nodes = new();
            CreateMap(nodes);

            MapPositions mapPositions = SetupInput();
            //MapPositions mapPositions = SetupExample();

            MapPositions best = null;
            LeastEnergy(mapPositions, ref best, maxDepthAllowed);
            Console.WriteLine("Part 1: " + best.Cost);
        }
        
        private static MapPositions SetupInput()
        {
            char[] startingPositions = new char[19];
            startingPositions[11] = 'B';
            startingPositions[12] = 'D';
            startingPositions[13] = 'A';
            startingPositions[14] = 'A';
            startingPositions[15] = 'B';
            startingPositions[16] = 'D';
            startingPositions[17] = 'C';
            startingPositions[18] = 'C';
            return new MapPositions(startingPositions);
        }
        private static MapPositions SetupExample()
        {
            char[] startingPositions = new char[19];
            startingPositions[11] = 'B';
            startingPositions[12] = 'A';
            startingPositions[13] = 'C';
            startingPositions[14] = 'D';
            startingPositions[15] = 'B';
            startingPositions[16] = 'C';
            startingPositions[17] = 'D';
            startingPositions[18] = 'A';
            return new MapPositions(startingPositions);
        }
        
        private static Node AnyHasToMove(MapPositions mapPositions)
        {
            for (int i = 0; i < mapPositions.Layout.Length; i++)
                if (mapPositions.Layout[i] != default && !nodes[i].CanStop)
                    return nodes[i];
            return null;
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
            
            //Check if there is one that we HAVE to move
            var hasToMove = AnyHasToMove(mapPositions);
            if (hasToMove != null)
            {
                List<Node> legalMoves = GetLegalMoves(hasToMove, mapPositions);
                if (legalMoves == null || legalMoves.Count == 0) //No solution found
                    return;
                
                foreach (var move in legalMoves)
                {
                    //MapPositions newPositions = new MapPositions(mapPositions, hasToMove.Id, move.Id);
                    //LeastEnergy(newPositions, ref best, remainingAllowedMoves - 1);
                    moves.Add(new MapPositions(mapPositions, hasToMove.Id, move.Id));
                }
            }
            else
            {
                for (int i = 0; i < mapPositions.Layout.Length; i++)
                {
                    if (mapPositions.Layout[i] == default)
                        continue;

                    List<Node> legalMoves = GetLegalMoves(nodes[i], mapPositions);
                    if (legalMoves == null) //No solution found
                        continue;
                    
                    foreach (var move in legalMoves)
                    {
                        //MapPositions newPositions = new MapPositions(mapPositions, i, move.Id);
                        //LeastEnergy(newPositions, ref best, remainingAllowedMoves - 1);
                        
                        moves.Add(new MapPositions(mapPositions, i, move.Id));
                    }
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

        private static List<Node> GetLegalMoves(Node node, MapPositions mapPositions)
        {
            var letterToMove = mapPositions.Layout[node.Id];
            var caveEntrance = nodes.First(n => n.EndLocation == letterToMove && n.Connections.Count == 2);
            var caveBottom = caveEntrance.Connections.First(n => n.EndLocation == letterToMove);
            
            if (node == caveBottom)
            {
                //We are in correct spot
                return null;
            }
            
            if (node == caveEntrance && mapPositions.Layout[caveBottom.Id] == letterToMove)
            {
                //Cave is completed. We are in correct spot
                return null;
            }

            var nodeInfrontOfCave = caveEntrance.Connections.First(n => n.EndLocation == default);
            bool inHallway = node.EndLocation == default;
            List<Node> allowedMoves = new List<Node>();

            if (inHallway)
            {
                if (mapPositions.Layout[caveEntrance.Id] == default)
                {
                    var freepasageToCave = true;
                    var dir = nodeInfrontOfCave.Id > node.Id ? 1 : -1;
                    for (int i = node.Id; i != nodeInfrontOfCave.Id; i += dir)
                        if (i != node.Id && mapPositions.Layout[i] != default)
                            freepasageToCave = false;

                    if (freepasageToCave)
                    {
                        if (mapPositions.Layout[caveBottom.Id] == default)
                        {
                            allowedMoves.Add(caveBottom); //We can move to the bottom of the cave
                        }
                        else if (mapPositions.Layout[caveBottom.Id] == letterToMove)
                        {
                            allowedMoves.Add(caveEntrance); //We can move to the first spot in the cave
                        }
                    }
                }

                if (!node.CanStop) //We have just moved into the hallway and has to move. So we can also move to any available position in the hallway
                {
                    for (int i = node.Id - 1; i >= 0; i--)
                    {
                        if (mapPositions.Layout[i] != default) //Blocked, we can not move further in this direction
                            break;
                        if (nodes[i].CanStop)
                            allowedMoves.Add(nodes[i]);
                    }
                    for (int i = node.Id + 1; i <= 10; i++)
                    {
                        if (mapPositions.Layout[i] != default) //Blocked, we can not move further in this direction
                            break;
                        if (nodes[i].CanStop)
                            allowedMoves.Add(nodes[i]);
                    }
                }
            }
            else
            {
                //We are in a cave, but it's either not the correct cave or there is another wrong letter in the cave. See if we can move out
                if (node.Connections.Count == 1 && mapPositions.Layout[node.Connections[0].Id] == default) //Bottom of cave
                    allowedMoves.Add(node.Connections[0].Connections.First(n => n.EndLocation == default));
                else if (node.Connections.Count == 2)//Front of cave
                    allowedMoves.Add(node.Connections.First(n => n.EndLocation == default));
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
            Node caveA1 = new Node(endLocation, true);
            Node caveA2 = new Node(endLocation, true);
            caveA1.Connections.Add(nodes[connectionToHallway]);
            caveA1.Connections.Add(caveA2);
            caveA2.Connections.Add(caveA1);
            nodes.Add(caveA1);
            nodes.Add(caveA2);
            
            nodes[connectionToHallway].Connections.Add(caveA1);
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
            Console.WriteLine("#############");
            Console.Write("#");
            for (int i = 0; i <= 10; i++)
                Console.Write(GetPrintLetter(i));
            Console.WriteLine("#");
            Console.WriteLine($"###{GetPrintLetter(11)}#{GetPrintLetter(13)}#{GetPrintLetter(15)}#{GetPrintLetter(17)}###");
            Console.WriteLine($"  #{GetPrintLetter(12)}#{GetPrintLetter(14)}#{GetPrintLetter(16)}#{GetPrintLetter(18)}#  ");
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
            if (a <= 10 && b <= 10)
            {
                return Math.Abs(a - b);
            }

            if (a <= 10)
            {
                if (b == 11)
                    return Math.Abs(a - 2) + 1;
                if (b == 12)
                    return Math.Abs(a - 2) + 2;
                
                if (b == 13)
                    return Math.Abs(a - 4) + 1;
                if (b == 14)
                    return Math.Abs(a - 4) + 2;
                
                if (b == 15)
                    return Math.Abs(a - 6) + 1;
                if (b == 16)
                    return Math.Abs(a - 6) + 2;
                
                if (b == 17)
                    return Math.Abs(a - 8) + 1;
                if (b == 18)
                    return Math.Abs(a - 8) + 2;
            }
            
            if (b <= 10)
            {
                if (a == 11)
                    return Math.Abs(b - 2) + 1;
                if (a == 12)
                    return Math.Abs(b - 2) + 2;
                
                if (a == 13)
                    return Math.Abs(b - 4) + 1;
                if (a == 14)
                    return Math.Abs(b - 4) + 2;
                
                if (a == 15)
                    return Math.Abs(b - 6) + 1;
                if (a == 16)
                    return Math.Abs(b - 6) + 2;
                
                if (a == 17)
                    return Math.Abs(b - 8) + 1;
                if (a == 18)
                    return Math.Abs(b - 8) + 2;
            }

            throw new Exception("Case not known: Moving from " + a + " to " + b);
        }
    }
}