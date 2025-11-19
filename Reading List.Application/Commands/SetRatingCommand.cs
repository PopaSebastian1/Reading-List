using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;

namespace Reading_List.Application.Commands
{
    public class SetRatingCommand : ICommand
    {
        public string Key => "7";
        public string Description => "Rate a Book";

        private readonly IBookService _bookService;

        public SetRatingCommand(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var id = ConsoleInputHandler.ReadInt("Enter the Book ID to rate: ", i => i > 0, ct);

            var rating = ConsoleInputHandler.ReadDecimal("Enter your rating (1-5): ", r => r >= 1 && r <= 5, ct);

            var result = await _bookService.SetRating(id, rating);

            if (result.IsSuccess)
            {
                return $"Book {id} rated {rating:0.0}/5 -> 200 OK";
            }
            else if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return $"Book {id} not found -> 404 Not Found";
            }
            else
            {
                return $"Failed to rate book {id}: {result.ErrorMessage} -> 400 Bad Request";
            }

        }
    }
}
