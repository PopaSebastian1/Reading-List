using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class ClearConsoleCommand : ICommand
    {
        public string Key => "C";
        public string Description => "Clear Console";
        public Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            Console.Clear();
            return Task.FromResult("Console cleared.");
        }
    }
}
