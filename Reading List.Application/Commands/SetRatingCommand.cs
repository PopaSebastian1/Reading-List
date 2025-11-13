using Reading_List.Application.Abstractions;

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
        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.Write("Enter the Book ID to rate: ");
            int id = Int32.TryParse(Console.ReadLine(), out var bookId) ? bookId : 0;

            if (id == 0)
            {
                Console.WriteLine("Invalid Book ID format.");
                return;
            }

            Console.Write("Enter your rating (1-5): ");

            decimal rating = decimal.TryParse(Console.ReadLine(), out var bookRating) ? bookRating : 0;

            if (rating < 1 || rating > 5)
            {
                Console.WriteLine("Rating must be between 1 and 5.");
                return;
            }

            var result = await _bookService.SetRating(id, rating, ct);
            if (result.IsSuccess)
            {
                Console.WriteLine($"Book {id} marked as finished -> 200 OK");
            }
            else
            {
                if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    Console.WriteLine($"Book {id} not found -> 404 Not Found");
                else
                    Console.WriteLine($"Error rating book {id}: {result.ErrorMessage} -> 400 Bad Request");
            }
        }
    }
}
