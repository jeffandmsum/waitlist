using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassPresentationGoFirstinator
{
    class Program
    {
        private static Random random = new Random();
        private static List<string> ListOfGroups = new List<string>()
        {
            "404: Team Name Not Found",
            "Dragons",
            "Group 8",
            "Little Bits",
            "Macrohard",
            "Null Pointer Exception",
            "Software Engineers"
        };

        static void Main(string[] args)
        {
            GenerateGroupOrder();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void GenerateGroupOrder()
        {
            Console.WriteLine("Here are the groups I know about:");
            for (int i = 0; i < ListOfGroups.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {ListOfGroups[i]}");
            }

            List<int> finalOrder = new List<int>();
            Console.WriteLine("Have any groups volunteered to go first/next? If so, which group number? (If not, select 'n')");
            string firstVolunteer = Console.ReadLine();

            while (firstVolunteer != "n" && finalOrder.Count < ListOfGroups.Count)
            {
                int nextGroup = 0;

                if (!int.TryParse(firstVolunteer, out nextGroup))
                {
                    Console.WriteLine("Either a group number or n please");
                    firstVolunteer = Console.ReadLine();
                }
                else if (finalOrder.Contains(nextGroup))
                {
                    Console.WriteLine("They already volunteered, try again");
                    firstVolunteer = Console.ReadLine();
                }
                else if (nextGroup <= 0 || nextGroup > ListOfGroups.Count)
                {
                    Console.WriteLine("That number makes no sense, try again");
                    firstVolunteer = Console.ReadLine();
                }
                else
                {
                    finalOrder.Add(nextGroup);
                    Console.WriteLine("Any more? (Enter either a group number or 'n')");
                    firstVolunteer = Console.ReadLine();
                }
            }

            if (finalOrder.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Here is the current order of group volunteers:");
                for (int i = 0; i < finalOrder.Count; i++)
                {
                    Console.WriteLine($"Group {finalOrder[i]} ({ListOfGroups[finalOrder[i] - 1]})");
                }
            }

            if (finalOrder.Count < ListOfGroups.Count)
            {
                Console.WriteLine();
                Console.WriteLine("Let's randomize the order of the rest, since humans are bad at randomizing things");

                List<int> remainingItems = new List<int>();
                for (int i = 0; i < ListOfGroups.Count; i++)
                {
                    if (!finalOrder.Contains(i + 1))
                    {
                        remainingItems.Add(i);
                    }
                }

                for (int i = 0; i < 200; i++)
                {
                    RandomizeList(remainingItems);
                    string randomString = string.Join(", ", remainingItems.Select(x => x + 1));
                    ClearLine();
                    Console.Write($"  Randomizing: {randomString}");
                    Thread.Sleep(5);
                }
                Console.WriteLine();

                for (int i = 0; i < remainingItems.Count; i++)
                {
                    Console.WriteLine($"Group {remainingItems[i] + 1} ({ListOfGroups[remainingItems[i]]})");
                }
            }
            else
            {
                Console.WriteLine("Looks like everyone volunteered, great, I'm useless then.");
            }
        }

        private static void RandomizeList(List<int> theList)
        {
            // Shuffle using the Fisher-Yates algorithm
            for (int i = 0; i < theList.Count - 1; i++)
            {
                int randomIndex = i + random.Next(theList.Count - i);

                int currentValue = theList[i];
                theList[i] = theList[randomIndex];
                theList[randomIndex] = currentValue;
            }
        }

        public static void WriteTopStatus(string message)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            string blankString = new string(' ', Console.BufferWidth);
            Console.SetCursorPosition(0, 0);
            Console.Write(blankString);

            Console.SetCursorPosition(0, 0);
            Console.Write(message.Substring(0, Math.Min(message.Length, Console.BufferWidth)));

            Console.SetCursorPosition(currentLeft, currentTop);
        }

        public static void WriteColor(string text, ConsoleColor foregroundColor)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ForegroundColor = defaultColor;
        }

        public static void ClearLine()
        {
            string blankString = new string(' ', Console.BufferWidth);
            Console.CursorLeft = 0;
            Console.Write($"{blankString}");
            Console.CursorTop--;
        }
    }
}
