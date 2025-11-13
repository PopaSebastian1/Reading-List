using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class GetTopRatedCommand : ICommand
    {
        public string Key => "3";
        public string Description => "Get Top Rated Books";

        private readonly IBookService _bookService;

        public GetTopRatedCommand(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.Write("Enter number of top rated books to retrieve: ");
            var input = Int32.TryParse(Console.ReadLine(), out var count) ? count : 5;
            var topBooksResult = await _bookService.GetTopRatedBooks(count);
            Console.WriteLine("Top Rated Books:");
            foreach (var result in topBooksResult)
            {
                if (result.IsSuccess && result.Value != null)
                {
                    var book = result.Value;
                    Console.WriteLine($"{book.ToString()}");
                }
                else
                {
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }
            }
        }
    }

}