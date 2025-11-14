using Reading_List.Application.Abstractions;

namespace Reading_List.CLI
{
    public sealed class Menu
    {
        private readonly IEnumerable<ICommand> _commands;

        public Menu(IEnumerable<ICommand> commands)
        {
            _commands = commands.OrderBy(c => c.Key).ToList();
        }

        private void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== Reading List Menu ===");
            foreach (var cmd in _commands)
                Console.WriteLine($"{cmd.Key}. {cmd.Description}");
            Console.WriteLine("X. Exit");

        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            PrintMenu();
            Console.Write("Choose option: ");
            while (!ct.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid option.");
                    continue;
                }

                if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                    break;

                var command = _commands.FirstOrDefault(c => c.Key.Equals(input, StringComparison.OrdinalIgnoreCase));
                if (command is null)
                {
                    Console.WriteLine("Unknown option.");
                    continue;
                }

                try
                {
                    await command.ExecuteAsync(ct);
                    if (command.Key == "C")
                        PrintMenu();
                    Console.Write("Choose option: ");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }

            Console.WriteLine("Goodbye!");
        }
    }
}