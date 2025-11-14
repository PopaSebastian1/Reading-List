using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;

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
            var count = ConsoleInputHandler.ReadInt("Enter number of top rated books to retrieve: ", v => v > 0);
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