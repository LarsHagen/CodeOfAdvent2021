using System;

namespace Day21
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Player player1 = new Player() {position = 8, score = 0};
            Player player2 = new Player() {position = 4, score = 0};
            DeterministicDice dice = new DeterministicDice();

            while (player1.score < 1000 && player2.score < 1000)
            {
                player1.Move(dice.Roll() + dice.Roll() + dice.Roll());
                if (player1.score >= 1000)
                    Console.WriteLine("Part 1: " + player2.score * dice.totalRolls);
                
                player2.Move(dice.Roll() + dice.Roll() + dice.Roll());
                if (player2.score >= 1000)
                    Console.WriteLine("Part 1: " + player1.score * dice.totalRolls);
            }

            new Part2().Run();

        }
    }

    public class Player
    {
        public ulong position;
        public ulong score;

        public void Move(ulong distance)
        {
            position += distance;
            while (position > 10)
                position -= 10;

            score += position;
        }
    }

    public class DeterministicDice
    {
        public ulong totalRolls { get; private set; }
        private ulong roll = 0;

        public ulong Roll()
        {
            totalRolls++;
            
            roll++;
            if (roll > 100)
                roll = 1;

            return roll;
        }
    }

}