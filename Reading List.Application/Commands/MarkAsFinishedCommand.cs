using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;

namespace Reading_List.Application.Commands
{
    public class MarkAsFinishedCommand : ICommand
    {
        public string Key => "6";
        public string Description => "Mark a Book as Finished";

        private readonly IBookService _bookService;

        public MarkAsFinishedCommand(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var id = ConsoleInputHandler.ReadInt("Enter the ID of the book to mark as finished: ", i => i > 0, ct);

            var result = await _bookService.MarkAsFinished(id);

            if (result.IsSuccess)
            {
                Console.WriteLine($"Book {id} marked as finished -> 200 OK");
            }
            else if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                Console.WriteLine($"Book {id} not found -> 404 Not Found");
            }
            else
            {
                Console.WriteLine($"Failed to mark book {id} as finished -> 500 Other error: {result.ErrorMessage}");
            }

            Console.ResetColor();
        }
    }
}
