using Reading_List.Application.Abstractions;

namespace Reading_List.CLI
{
    public sealed class MenuWithoutCommands
    {
        private readonly WithoutCommands _logic;

        public MenuWithoutCommands(IBookService bookService, IBookImportHandler importHandler)
        {
            _logic = new WithoutCommands(bookService, importHandler);
        }

        private static void PrintMenu()
        {
            Console.WriteLine("""
                    === Mini Reading List Menu ===
                    0. Import default CSV (books_part1.csv)
                    1. List all books
                    2. List top rated books
                    3. List finished books
                    4. Show stats
                    X. Exit
                    """);
            Console.Write("Choose option: ");
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            Console.WriteLine(await _logic.ImportDefaultAsync(ct));

            PrintMenu();
            while (!ct.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("Invalid. Choose option: ");
                    continue;
                }
                if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    switch (input)
                    {
                        case "0":
                            Console.WriteLine(await _logic.ImportDefaultAsync(ct));
                            break;
                        case "1":
                            await _logic.ListAllAsync(ct);
                            break;
                        case "2":
                            var count = ReadInt("How many top rated (default 5)? ", v => v > 0, defaultValue: 5);
                            await _logic.ListTopRatedAsync(count, ct);
                            break;
                        case "3":
                            await _logic.ListFinishedAsync(ct);
                            break;
                        case "4":
                            await _logic.ShowStatsAsync(ct);
                            break;
                        default:
                            Console.WriteLine("Unknown option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.Write("Choose option: ");
            }

            Console.WriteLine("Goodbye (mini mode)!");
        }

        private static int ReadInt(string prompt, Func<int, bool> predicate, int defaultValue)
        {
            Console.Write(prompt);
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return defaultValue;

            if (int.TryParse(line, out var value) && predicate(value))
                return value;

            Console.WriteLine($"Invalid value. Using default {defaultValue}.");
            return defaultValue;
        }
    }
}