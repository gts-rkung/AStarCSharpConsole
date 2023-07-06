namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHelp();
            FindPathAStar astar = new FindPathAStar();
            while (true)
            {
                var key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key == ConsoleKey.R)
                {
                    astar.BeginSearch();
                }
                else if (key == ConsoleKey.C)
                {
                    astar.Search();
                }
                else if (key == ConsoleKey.H)
                {
                    PrintHelp();
                }
                else if (key == ConsoleKey.Q)
                {
                    return;
                }
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("=== A* Path Finding ===");
            Console.WriteLine("Press R to Restart");
            Console.WriteLine("Press C to Continue");
            Console.WriteLine("Press Q to Quit");
            Console.WriteLine("Press H for Help");
        }
    }
}
