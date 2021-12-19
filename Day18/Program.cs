using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //INumber number = Parse("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]");
            
            var lines = File.ReadAllLines("Input.txt");
            
            INumber number = Parse(lines[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                number = Add(number, Parse(lines[i]));
                Reduce(number);
            }
            
            Console.WriteLine("Part 1: " + number.Magnitude());

            int largestMagnitude = 0;
            
            for (int a = 0; a < lines.Length; a++)
            {
                for (int b = 0; b < lines.Length; b++)
                {
                    if (a == b)
                        continue;

                    INumber sum = Add(Parse(lines[a]), Parse(lines[b]));
                    Reduce(sum);
                    if (sum.Magnitude() > largestMagnitude)
                    {
                        largestMagnitude = sum.Magnitude();
                    }
                }
            }
            
            Console.WriteLine("Part 2: " + largestMagnitude);
        }

        private static void Reduce(INumber number)
        {
            bool anythingChanged;
            do
            {
                anythingChanged = false;
                if (number.Explode())
                    anythingChanged = true;
                else if (number.Split())
                    anythingChanged = true;
            } while (anythingChanged);
        }

        private static INumber Add(INumber a, INumber b)
        {
            return new SnailfishNumber(a, b);
        }

        private static INumber Parse(string input)
        {
            if (input.First() == '[')
            {
                //Strip outer layer of []
                input = input.Substring(1, input.Length - 2);

                int splitIndex = 0;
                int depth = 0;
                for (var i = 0; i < input.Length; i++)
                {
                    var c = input[i];
                    if (c == ',' && depth == 0)
                    {
                        splitIndex = i;
                        break;
                    }

                    if (c == '[')
                        depth++;
                    if (c == ']')
                        depth--;
                }

                return new SnailfishNumber(
                    Parse(input.Substring(0, splitIndex)),
                    Parse(input.Substring(splitIndex + 1)));
            }

            return new RegularNumber(int.Parse(input));
        }
    }
    
    public class SnailfishNumber : INumber
    {
        private INumber left;
        private INumber right;

        public SnailfishNumber(INumber left, INumber right)
        {
            this.left = left;
            this.right = right;

            left.Parent = this;
            right.Parent = this;
        }

        public INumber Parent { get; set; }

        public bool Split()
        {
            if (left.Split())
                return true;

            if (right.Split())
                return true;

            return false;
        }
        
        public bool Explode(int depth = 0)
        {
            if (depth > 3)
            {
                Parent.AddLeft(((RegularNumber)left).value, this);
                Parent.AddRight(((RegularNumber)right).value, this);
                ((SnailfishNumber)Parent).Replace(this, new RegularNumber(0));
                return true;
            }

            if (left.Explode(depth + 1))
                return true;
            if (right.Explode(depth + 1))
                return true;

            return false;
        }

        public int Magnitude()
        {
            return 3 * left.Magnitude() + 2 * right.Magnitude();
        }
        
        public void AddLeft(int value, INumber sender)
        {
            if (left == sender && Parent != null)
                ((SnailfishNumber)Parent).AddLeft(value, this);
            
            if (right == sender) //Switch
                left.AddRight(value, this);
            else
                left.AddLeft(value, this);
        }
        public void AddRight(int value, INumber sender)
        {
            if (right == sender && Parent != null)
                ((SnailfishNumber)Parent).AddRight(value, this);
            
            if (left == sender) //Switch
                right.AddLeft(value, this);
            else
                right.AddRight(value, this);
        }

        public void Replace(INumber old, INumber value)
        {
            if (left == old)
                left = value;
            else if (right == old)
                right = value;
            else
                throw new Exception("INumber not found");

            value.Parent = this;
        }

        public string ToString()
        {
            return $"[{left.ToString()},{right.ToString()}]";
        }
    }

    public interface INumber
    {
        INumber Parent { get; set; }
        bool Explode(int depth = 0);
        bool Split();
        void AddLeft(int value, INumber sender);
        void AddRight(int value, INumber sender);
        string ToString();
        int Magnitude();
    }

    public class RegularNumber : INumber
    {
        public int value;
        public INumber Parent { get; set; }

        public RegularNumber(int value)
        {
            this.value = value;
        }

        public int Magnitude()
        {
            return value;
        }
        
        public bool Explode(int depth = 0)
        {
            return false;
        }
       
        public void AddLeft(int value, INumber sender)
        {
            this.value += value;
        }

        public void AddRight(int value, INumber sender)
        {
            this.value += value;
        }

        public bool Split()
        {
            if (value <= 9)
                return false;
            
            if (Parent is SnailfishNumber snailfishNumber)
            {
                snailfishNumber.Replace(this,
                    new SnailfishNumber(
                        new RegularNumber(value / 2),
                        new RegularNumber((int)Math.Ceiling(value/2.0))));
                
                
                return true;
            }

            throw new Exception("Parent is not a snailFishNumber. This should not be possible");
        }

        public string ToString()
        {
            return value.ToString();
        }
    }
}