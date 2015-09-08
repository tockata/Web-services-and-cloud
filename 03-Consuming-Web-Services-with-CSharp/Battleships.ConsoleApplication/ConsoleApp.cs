namespace Battleships.ConsoleApplication
{
    using System;

    public class ConsoleApp
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to Battleships game!");
            Console.WriteLine("Please enter command with parameters to play.");
            Console.WriteLine("You can always exit the game by typing 'end' and pressing Enter.");
            string line = Console.ReadLine();
            while (line != "end")
            {
                GameEngine.ParseCommand(line);
                line = Console.ReadLine();
            }
        }
    }
}
