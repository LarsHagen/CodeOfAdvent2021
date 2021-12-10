using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IList<string> input = File.ReadLines("Input.txt").ToList();

            List<int> numbers = input[0].Split(',').Select(int.Parse).ToList();

            input.RemoveAt(0);
            input.RemoveAt(0);

            List<Board> boards = new List<Board>();

            for (int i = 0; i <= input.Count / 6; i++)
            {
                int start = i * 6;
                boards.Add(new Board(input[start + 0], input[start + 1], input[start + 2], input[start + 3], input[start + 4]));
            }

            Console.WriteLine("Part 1: " + Part1(numbers, boards));
            Console.WriteLine();
            Console.WriteLine("Part 2: " + Part2(numbers, boards));
        }

        private static int Part1(List<int> numbers, List<Board> boards)
        {
            foreach (var number in numbers)
            {
                foreach (var board in boards)
                {
                    if (board.AddNumber(number))
                    {
                        Console.WriteLine("Winner board: ");
                        board.Print();
                        Console.WriteLine("Last number was " + number);
                        return board.SumOfUnmarkedNumbers() * number;
                    }
                }
            }

            throw new Exception("Something went wrong and no winner was found");
        }
        
        private static int Part2(List<int> numbers, List<Board> boards)
        {
            foreach (var number in numbers)
            {
                foreach (var board in boards.ToArray())
                {
                    if (board.AddNumber(number))
                    {
                        if (boards.Count == 1)
                        {
                            Console.WriteLine("Last board to win:");
                            boards[0].Print();
                            Console.WriteLine("Last number was " + number);
                            return board.SumOfUnmarkedNumbers() * number;
                        }
                        boards.Remove(board);
                    }
                }
            }

            throw new Exception("Something went wrong and no winner was found");
        }
    }

    internal class Board
    {
        public int[,] Values = new int[5, 5];
        public bool[,] Checked = new bool[5, 5];

        public Board(params string[] rows)
        {
            for (int y = 0; y < 5; y++)
            {
                var parsedRow = rows[y].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                for (int x = 0; x < 5; x++)
                {
                    Values[x, y] = parsedRow[x];
                }
            }
        }

        public bool AddNumber(int input)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (Values[x, y] == input)
                    {
                        Checked[x, y] = true;
                        if (BingoAtCoordinate(x, y))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool BingoAtCoordinate(int x, int y)
        {
            bool bingoRow = Checked[0, y] && Checked[1, y] && Checked[2, y] && Checked[3, y] && Checked[4, y];
            bool bingoColumn = Checked[x, 0] && Checked[x, 1] && Checked[x, 2] && Checked[x, 3] && Checked[x, 4];

            return bingoRow || bingoColumn;
        }

        public int SumOfUnmarkedNumbers()
        {
            int sum = 0;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (!Checked[x, y])
                        sum += Values[x, y];
                }
            }

            return sum;
        }

        public void Print()
        {
            Console.WriteLine("Board print:");
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Console.ForegroundColor = Checked[x, y] ? ConsoleColor.Green : ConsoleColor.Gray;
                    var value = Values[x, y];
                    if (value < 10)
                        Console.Write(" " + value + " ");
                    else
                        Console.Write(value + " ");

                }
                Console.WriteLine();
                Console.ResetColor();
            }
        }
    }
}