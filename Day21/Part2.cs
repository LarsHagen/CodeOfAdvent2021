using System;
using System.Collections.Generic;

namespace Day21
{
    public class Part2
    {
        private Dictionary<(int player1Position, int player2Position, int player1Score, int player2Score), ulong> situations = new();
        public void Run()
        {
            situations.Add(new(8, 4, 0, 0), 1);

            bool gameWasPlayed = true;
            while (gameWasPlayed)
            {
                gameWasPlayed = false;
                if (PlayTurn(true))
                    gameWasPlayed = true;
                if (PlayTurn(false))
                    gameWasPlayed = true;
            }

            ulong p1Win = 0;
            ulong p2Win = 0;
            foreach (var situation in situations)
            {
                if (situation.Key.player1Score >= 21)
                    p1Win += situation.Value;
                if (situation.Key.player2Score >= 21)
                    p2Win += situation.Value;
            }
            
            Console.WriteLine("Player 1 wins: " + p1Win);
            Console.WriteLine("Player 2 wins: " + p2Win);
            Console.WriteLine("Part 2: " + Math.Max(p1Win, p2Win));
        }

        private bool PlayTurn(bool player1Turn)
        {
            bool gameWasPlayed = false;
            Dictionary<(int player1Position, int player2Position, int player1Score, int player2Score), ulong> temp = new();

            foreach (var situation in situations)
            {
                if (situation.Value == 0)
                    continue;
                if (situation.Key.player1Score >= 21 || situation.Key.player2Score >= 21)
                {
                    if (!temp.ContainsKey(situation.Key))
                        temp.Add(situation.Key, 0);
                    temp[situation.Key] += situation.Value;
                    continue;
                }

                gameWasPlayed = true;
                foreach (var roll in _possibleRolls)
                {
                    var newPosition = player1Turn ? situation.Key.player1Position : situation.Key.player2Position;
                    newPosition += roll;
                    if (newPosition > 10)
                        newPosition -= 10;

                    (int player1Position, int player2Position, int player1Score, int player2Score) newSituation = player1Turn
                        ? new(newPosition, situation.Key.player2Position,
                            situation.Key.player1Score + newPosition, situation.Key.player2Score)
                        : new (situation.Key.player1Position, newPosition,
                            situation.Key.player1Score, situation.Key.player2Score + newPosition);

                    if (!temp.ContainsKey(newSituation))
                        temp.Add(newSituation, 0);
                    temp[newSituation] += situation.Value;
                }
            }

            situations = temp;
            return gameWasPlayed;
        }

        private readonly int[] _possibleRolls = new[]
        {
            (1 + 1 + 1), (1 + 1 + 2), (1 + 1 + 3),
            (1 + 2 + 1), (1 + 2 + 2), (1 + 2 + 3),
            (1 + 3 + 1), (1 + 3 + 2), (1 + 3 + 3),
            (2 + 1 + 1), (2 + 1 + 2), (2 + 1 + 3),
            (2 + 2 + 1), (2 + 2 + 2), (2 + 2 + 3),
            (2 + 3 + 1), (2 + 3 + 2), (2 + 3 + 3),
            (3 + 1 + 1), (3 + 1 + 2), (3 + 1 + 3),
            (3 + 2 + 1), (3 + 2 + 2), (3 + 2 + 3),
            (3 + 3 + 1), (3 + 3 + 2), (3 + 3 + 3)
        };
    }
}