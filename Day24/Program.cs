using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day24
{
    class Program
    {
        static int _w = 0;
        static int _x = 0;
        static int _y = 0;
        static int _z = 0;
        static int _input = 0;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var lines = File.ReadAllLines("Input.txt");
            

            Dictionary<string, Instruction> instructions = new();
            instructions.Add("inp", Inp);
            instructions.Add("add", Add);
            instructions.Add("mul", Mul);
            instructions.Add("div", Div);
            instructions.Add("mod", Mod);
            instructions.Add("eql", Eql);

            List<List<Action>> instructionBlocks = new();
            List<Action> currentInstructionBlock = new();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.Contains("inp"))
                {
                    currentInstructionBlock = new();
                    instructionBlocks.Add(currentInstructionBlock);
                    currentInstructionBlock.Add(() => Inp(ref GetInt(line[4]), ref _input));
                }
                else
                {
                    var instruction = instructions[line.Substring(0, 3)];
                    
                    if (int.TryParse(line.Substring(6), out var b))
                    {
                        currentInstructionBlock.Add(() => instruction.Invoke(ref GetInt(line[4]), ref b));
                    }
                    else
                    {
                        currentInstructionBlock.Add(() => instruction.Invoke(ref GetInt(line[4]), ref GetInt(line[6])));
                    }
                }
            }

            
            //x is always set to the value of z
            //x is always mod 26, so it will always end up being a value between 0 and 25
            //z is always divided by 26
            //y is always set to 25
            //This means that z is the only value that matters to bring over to next instruction block

            //Lets get an idea for what types of input on z gives a valid model number on the last instruction
            var validOnLast = GetValidInputsOnBlock(instructionBlocks[13], new List<int>() {0});
            
            //Valid outputs from above seems to be:
            // i = 1, z = 3
            // i = 2, z = 4
            // i = 3, z = 5
            // i = 4, z = 6
            // i = 5, z = 7
            // i = 6, z = 8
            // i = 7, z = 9
            // i = 8, z = 10
            // i = 9, z = 11
            // So z is valid if it's two higher then i
            
            //Go backwards through the blocks using the valid inputs in the next block in the list to check output against
            //To then find valid w and z values for this block
            Dictionary<int, List<(int i,int zIn, int _zOut)>> validInputsForEachBlock = new();
            validInputsForEachBlock.Add(13, validOnLast.ToList());

            for (int i = 12; i >= 0; i--)
            {
                Console.WriteLine();
                Console.WriteLine("Block " + i);
                validOnLast =
                    GetValidInputsOnBlock(instructionBlocks[i], validOnLast.Select(entry => entry.zIn).ToList());
                validInputsForEachBlock.Add(i, validOnLast.ToList());
            }
            
            //Now we remove valid inputs we can't reach. Starting with block 0 we remove all inputs where z != 0 (since z has to start as zero)
            validInputsForEachBlock[0].RemoveAll(i => i.zIn != 0);
            
            //Then we go over each valid input for each block and remove values where z cannot be reached in previous block
            for (int i = 1; i <= 13; i++)
            {
                var zValuesWeCanGetInPreviousBlock = validInputsForEachBlock[i - 1].Select(i => i._zOut).ToList();
                validInputsForEachBlock[i].RemoveAll(i => !zValuesWeCanGetInPreviousBlock.Contains(i.zIn));
            }

            foreach (var v in validInputsForEachBlock)
            {
                Console.WriteLine("Block " + v.Key);
                foreach (var _ in v.Value)
                {
                    Console.WriteLine("i: " + _.i + " zIn: " + _.zIn + " zOut: " + _._zOut);
                }
            }

            int[] modelNoHigh = new int[14];
            int[] modelNoLow = new int[14];
            //For highest model no: Pick the last input (highest) in each validInputsForEachBlock
            //For lowest model no: Pick the first input (lowest) in each validInputsForEachBlock
            for (int i = 0; i < 14; i++)
            {
                modelNoHigh[i] = validInputsForEachBlock[i].Last().i;
                modelNoLow[i] = validInputsForEachBlock[i].First().i;
            }
            
            Console.WriteLine();
            
            //Part 1:
            Console.Write("Part 1: ");
            foreach (var i in modelNoHigh)
            {
                Console.Write(i);
            }
            Console.WriteLine();
            
            //Part 2:
            Console.Write("Part 2: ");
            foreach (var i in modelNoLow)
            {
                Console.Write(i);
            }
            Console.WriteLine();
        }

        private static List<(int i, int zIn, int zOut)> GetValidInputsOnBlock(List<Action> block, List<int> validZOutputs)
        {
            List<(int i, int zIn, int zOut)> validInputs = new();
            for (int i = 1; i <= 9; i++)
            {
                _input = i;
                for (int z = -10000; z <= 10000; z++)
                {
                    _z = z;
                    
                    ExecuteBlock(block);
                    if (validZOutputs.Contains(_z))
                    {
                        Console.WriteLine(
                            $"When input is {i} and z has initial value of {z}, then output z is {_z} (valid)");
                        
                        validInputs.Add((i,z, _z));
                    }
                }
            }

            return validInputs;
        }

        private static void ExecuteBlock(List<Action> instructions)
        {
            foreach (var instruction in instructions)
            {
                instruction.Invoke();
            }
        }

        private static ref int GetInt(char letter)
        {
            if (letter == 'w')
                return ref _w;
            if (letter == 'x')
                return ref _x;
            if (letter == 'y')
                return ref _y;
            if (letter == 'z')
                return ref _z;
            throw new Exception($"Not a valid letter '{letter}'. Allowed values are w, x, y or z");
        }

        private delegate void Instruction(ref int a, ref int b);

        private static void Inp(ref int a, ref int b) => a = b;
        private static void Add(ref int a, ref int b) => a += b;
        private static void Mul(ref int a, ref int b) => a *= b;
        private static void Div(ref int a, ref int b) => a /= b;
        private static void Mod(ref int a, ref int b) => a %= b;
        private static void Eql(ref int a, ref int b) => a = a == b ? 1 : 0;
    }
}