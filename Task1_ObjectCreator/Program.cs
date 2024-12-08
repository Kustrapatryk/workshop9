using System;

namespace Task1_ObjectCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Part 1
            InvokeCraft <Author>();


            // Part 2
            InvokeCraft <Book>();

        }

        private static void InvokeCraft<T>()
        {
            try
            {
                T obj = TypeCrafter.TypeCraft<T>();
                Console.WriteLine(obj);
            }
            catch(ParseException ex)
            {
                Console.WriteLine($"Wrong format for parsing. Attempting to craft object once again.");
                InvokeCraft <T>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //return null
        }
    }
}
