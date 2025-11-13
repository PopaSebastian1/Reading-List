using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class ClearConsoleCommand : ICommand
    {
        public string Key => "9";
        public string Description => "Clear Console";
        public Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.Clear();
            return Task.CompletedTask;
        }
    }
}
