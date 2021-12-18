using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string bits = "";
            var input = File.ReadAllText("Input.txt");
            foreach (var c in input)
            {
                bits += GetBits(c);
            }
            Console.WriteLine(bits);

            var package = Package.Parse(0, bits).package;
            Console.WriteLine("Part 1: " + package.GetVersionSum());
            Console.WriteLine("Part 2: " + package.GetValue());
        }

        private static string GetBits(char c)
        {
            return c switch
            {
                '0' => "0000",
                '1' => "0001",
                '2' => "0010",
                '3' => "0011",
                '4' => "0100",
                '5' => "0101",
                '6' => "0110",
                '7' => "0111",
                '8' => "1000",
                '9' => "1001",
                'A' => "1010",
                'B' => "1011",
                'C' => "1100",
                'D' => "1101",
                'E' => "1110",
                'F' => "1111"
            };
        }
    }

    public abstract class Package
    {
        public int version;
        public int type;

        public static (int usedBitsCount, Package package) Parse(int start, string fullBinaryString)
        {
            int type = ReadDecimal(start + 3, 3, fullBinaryString);

            //Package package = type == 4 ? new LiteralValuePackage() : new OperaterPackage();
            Package package = type switch
            {
                0 => new SumPackage(),
                1 => new ProductPackage(),
                2 => new MinimumPackage(),
                3 => new MaximumPackage(),
                4 => new LiteralValuePackage(),
                5 => new GreaterThenPackage(),
                6 => new LessThenPackage(),
                7 => new EqualPackage()
            };
            
            int usedBitsCount = package.ParseInstance(start, fullBinaryString);
            
            return (usedBitsCount, package);
        }

        protected abstract int ParseInstance(int start, string fullBinaryString);
        
        protected static int ReadDecimal(int start, int lenght, string bits)
        {
            return Convert.ToInt32(bits.Substring(start, lenght), 2);
        }

        public abstract int GetVersionSum();
        public abstract long GetValue();
    }

    public class LiteralValuePackage : Package
    {
        private long _value;
        
        protected override int ParseInstance(int start, string fullBinaryString)
        {
            version = ReadDecimal(start, 3, fullBinaryString);
            type = ReadDecimal(start + 3, 3, fullBinaryString);

            int j = start + 6;
            string binary = "";

            while (true)
            {
                binary += fullBinaryString.Substring(j + 1, 4);
                if (fullBinaryString[j] == '0')
                    break;
                j += 5;
            }

            _value = Convert.ToInt64(binary, 2);
            int usedBits = j - start + 5;

            return usedBits;
        }

        public override int GetVersionSum()
        {
            return version;
        }

        public override long GetValue()
        {
            return _value;
        }
    }

    public class SumPackage : OperaterPackage
    {
        public override long GetValue()
        {
            return GetValuesOfSubPackages().Sum();
        }
    }
    public class ProductPackage : OperaterPackage
    {
        public override long GetValue()
        {
            var vals = GetValuesOfSubPackages().ToArray();
            var result = vals[0];

            for (int i = 1; i < vals.Length; i++)
                result *= vals[i];

            return result;
        }
    }

    public class MinimumPackage : OperaterPackage
    {
        public override long GetValue()
        {
            return GetValuesOfSubPackages().Min();
        }
    }

    public class MaximumPackage : OperaterPackage
    {
        public override long GetValue()
        {
            return GetValuesOfSubPackages().Max();
        }
    }

    public class GreaterThenPackage : OperaterPackage
    {
        public override long GetValue()
        {
            var a = GetValuesOfSubPackages().First();
            var b = GetValuesOfSubPackages().Last();
            
            return a > b ? 1 : 0;
        }
    }
    public class LessThenPackage : OperaterPackage
    {
        public override long GetValue()
        {
            var a = GetValuesOfSubPackages().First();
            var b = GetValuesOfSubPackages().Last();
            
            return a < b ? 1 : 0;
        }
    }
    public class EqualPackage : OperaterPackage
    {
        public override long GetValue()
        {
            var a = GetValuesOfSubPackages().First();
            var b = GetValuesOfSubPackages().Last();
            
            return a == b ? 1 : 0;
        }
    }

    public abstract class OperaterPackage : Package
    {
        public List<Package> subPackages = new();

        protected override int ParseInstance(int start, string fullBinaryString)
        {
            version = ReadDecimal(start, 3, fullBinaryString);
            type = ReadDecimal(start + 3, 3, fullBinaryString);
            
            var lengthTypeId = fullBinaryString[start + 6] == '1' ? 11 : 15;
            int lengthTypeValue = ReadDecimal(start + 7, lengthTypeId, fullBinaryString);
            int usedBits = 0;
            int offset = start + lengthTypeId + 7;
            
            if (lengthTypeId == 11) //Lengthtype value is number of packages
            {
                
                for (int i = 0; i < lengthTypeValue; i++)
                {
                    var result = Parse(offset, fullBinaryString);
                    offset += result.usedBitsCount;
                    usedBits += result.usedBitsCount;
                    subPackages.Add(result.package);
                }
            }
            else
            {
                while (usedBits < lengthTypeValue)
                {
                    var result = Parse(offset, fullBinaryString);
                    offset += result.usedBitsCount;
                    usedBits += result.usedBitsCount;
                    subPackages.Add(result.package);
                }
            }
            usedBits += 7 + lengthTypeId;
            return usedBits;
        }

        public override int GetVersionSum()
        {
            int sum = version;
            foreach (var package in subPackages)
            {
                sum += package.GetVersionSum();
            }

            return sum;
        }

        protected IEnumerable<long> GetValuesOfSubPackages()
        {
            return subPackages.Select(s => s.GetValue());
        }
    }
}
