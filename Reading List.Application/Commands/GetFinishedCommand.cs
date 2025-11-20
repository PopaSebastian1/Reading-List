using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class GetFinishedCommand : ICommand
    {
        public string Key => "4";
        public string Description => "Get Finished Books";

        private readonly IBookService _bookService;

        public GetFinishedCommand(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var finishedResults = await _bookService.GetFinishedBooks();

            if (!finishedResults.Any())
            {
                return "No finished books found.";
            }

            return "Finished Books:\n" + string.Join("\n", finishedResults.Select(b => b.Value!.ToString()));
            
        }
    }
}
