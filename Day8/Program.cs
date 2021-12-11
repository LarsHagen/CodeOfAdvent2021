using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        //There has to be a better way to solve this then what I did 😂
        
        public static char[] allOptions = {'a', 'b', 'c', 'd', 'e', 'f', 'g'};
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = File.ReadAllLines("Input.txt");

            var part1Count = 0;
            
            foreach (var entry in lines)
            {
                var split = entry.Split(" | ");
                var signals = split[0].Split(" ");
                var outputs = split[1].Split(" ");
                part1Count += outputs.Count(x => x.Length == 2 || x.Length == 3 || x.Length == 4 || x.Length == 7);
            }
            
            Console.WriteLine("Part 1: " + part1Count);

            List<int> values = new();
            
            foreach (var entry in lines)
            {
                var split = entry.Split(" | ");
                var signals = split[0].Split(" ");
                var outputs = split[1].Split(" ");

                var one = signals.First(x => x.Length == 2);
                var four = signals.First(x => x.Length == 4);
                var seven = signals.First(x => x.Length == 3);
                var eight = signals.First(x => x.Length == 7);

                SignalDecode signalDecode = new();
                
                //First based on the signals we know remove possible mappings
                signalDecode.ProcessInputSignal(one, "cf");
                signalDecode.ProcessInputSignal(four, "bcdf");
                signalDecode.ProcessInputSignal(seven, "acf");
                signalDecode.ProcessInputSignal(eight, "abcdefg");
                signalDecode.PrintPossibilities();
                
                //we know what the two possible values are for C and F (even if we don't know what is what), so remove those possibilities from other mappings
                signalDecode.RemoveFromPossibleMapping('a', signalDecode.possibleMappings['c']);
                signalDecode.RemoveFromPossibleMapping('b', signalDecode.possibleMappings['c']);
                signalDecode.RemoveFromPossibleMapping('d', signalDecode.possibleMappings['c']);
                signalDecode.RemoveFromPossibleMapping('e', signalDecode.possibleMappings['c']);
                signalDecode.RemoveFromPossibleMapping('g', signalDecode.possibleMappings['c']);
                signalDecode.PrintPossibilities();
                
                //Now we know what A is mapped to, so remove that from the others
                signalDecode.RemoveFromPossibleMapping('b', signalDecode.possibleMappings['a']);
                signalDecode.RemoveFromPossibleMapping('d', signalDecode.possibleMappings['a']);
                signalDecode.RemoveFromPossibleMapping('e', signalDecode.possibleMappings['a']);
                signalDecode.RemoveFromPossibleMapping('g', signalDecode.possibleMappings['a']);
                signalDecode.PrintPossibilities();
                
                //And now we know the two possible values for B and D, so remove that from the others
                signalDecode.RemoveFromPossibleMapping('e', signalDecode.possibleMappings['b']);
                signalDecode.RemoveFromPossibleMapping('g', signalDecode.possibleMappings['b']);
                signalDecode.PrintPossibilities();

                //We can find zero now, we know the required letters to turn on A, C, E, F, G. zero will be the only 6 letter signal with those 5 values. The last letter in 6 will be the mapped value for B
                List<char> requiredInZero = new();
                requiredInZero.AddRange(signalDecode.possibleMappings['a']); //A we know
                requiredInZero.AddRange(signalDecode.possibleMappings['c']); //C and F (since we don't know what is what)
                requiredInZero.AddRange(signalDecode.possibleMappings['e']); //E and G (since we don't know what is what)
                var zero = signals.First(x => x.Length == 6 && requiredInZero.All(x.Contains));
                char b = zero.First(x => !requiredInZero.Contains(x));
                signalDecode.possibleMappings['b'].RemoveAll(x => x != b);
                
                //Now D must then be what we remove from B
                signalDecode.possibleMappings['d'].RemoveAll(x => x == b);
                signalDecode.PrintPossibilities();
                
                //We now know A, B, D so we can find 5 since it's the only signal with 5 chars and A, B, D
                List<char> requiredInFive = new();
                requiredInFive.AddRange(signalDecode.possibleMappings['a']);
                requiredInFive.AddRange(signalDecode.possibleMappings['b']);
                requiredInFive.AddRange(signalDecode.possibleMappings['d']);
                var five = signals.First(x => x.Length == 5 && requiredInFive.All(x.Contains));
                //The two chars in five that are not in the required list must be f and g
                var fg = five.Where(x => !requiredInFive.Contains(x)).ToList();
                //Remove that from c and e
                signalDecode.RemoveFromPossibleMapping('c', fg);
                signalDecode.RemoveFromPossibleMapping('e', fg);
                signalDecode.PrintPossibilities();
                
                //Now remove the known value of E from G and the known value of C from F
                signalDecode.RemoveFromPossibleMapping('g', signalDecode.possibleMappings['e']);
                signalDecode.RemoveFromPossibleMapping('f', signalDecode.possibleMappings['c']);
                signalDecode.PrintPossibilities();

                //Signals are mapped!

                string number = "";
                foreach (var output in outputs)
                {
                    number += signalDecode.DecodeUsingMappedValues(output);
                }
                Console.WriteLine(number);
                Console.WriteLine();
                
                values.Add(int.Parse(number));
            }
            
            Console.WriteLine("Part 2: " + values.Sum());
        }

        public class SignalDecode
        {
            public Dictionary<char, List<char>> possibleMappings = new();

            public SignalDecode()
            {
                
                possibleMappings.Add('a', allOptions.ToList());
                possibleMappings.Add('b', allOptions.ToList());
                possibleMappings.Add('c', allOptions.ToList());
                possibleMappings.Add('d', allOptions.ToList());
                possibleMappings.Add('e', allOptions.ToList());
                possibleMappings.Add('f', allOptions.ToList());
                possibleMappings.Add('g', allOptions.ToList());
            }

            private char Remap(char input)
            {
                return possibleMappings.First(x => x.Value.Contains(input)).Key;
            }

            public int DecodeUsingMappedValues(string input)
            {
                var remapped = "";
                foreach (var c in input)
                {
                    remapped += Remap(c);
                }
                return DecodeUsingStandardValues(remapped);
            }

            public int DecodeUsingStandardValues(string input)
            {
                input = String.Concat(input.OrderBy(c => c)); //Order alphabetically
                return input switch
                {
                    "abcefg" => 0,
                    "cf" => 1,
                    "acdeg" => 2,
                    "acdfg" => 3,
                    "bcdf" => 4,
                    "abdfg" => 5,
                    "abdefg" => 6,
                    "acf" => 7,
                    "abcdefg" => 8,
                    "abcdfg" => 9
                };
            }
            
            public void ProcessInputSignal(string input, string expected)
            {
                foreach (var c in expected)
                {
                    possibleMappings[c].RemoveAll(x => !input.Contains(x));
                }
            }

            public void RemoveFromPossibleMapping(char mapTo, IList<char> toRemove)
            {
                possibleMappings[mapTo].RemoveAll(toRemove.Contains);
            }

            public void PrintPossibilities()
            {
                foreach (var possibleMapping in possibleMappings)
                {
                    var stringToPrint = $"{possibleMapping.Key} = {new string(possibleMapping.Value.ToArray())}";
                    while (stringToPrint.Length < 20)
                        stringToPrint += " ";
                    Console.Write(stringToPrint);
                }
                Console.WriteLine();
            }
        }
    }
}