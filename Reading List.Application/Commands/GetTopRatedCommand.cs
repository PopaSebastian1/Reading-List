using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;

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
        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var count = ConsoleInputHandler.ReadInt("Enter number of top rated books to retrieve: ", v => v > 0);
            var topBooksResult = await _bookService.GetTopRatedBooks(count);
            var topBooks = new List<BookDto>();
            Console.WriteLine("Top Rated Books:");
            foreach (var result in topBooksResult)
            {
                if (result.IsSuccess && result.Value != null)
                {
                    var book = result.Value;
                    topBooks.Add(book);
                }
                else
                {
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }
            }
            return topBooks.Count > 0
                ? $"Top Rated Books:\n" + string.Join("\n", topBooks.Select(b => b.ToString()))
                : $"No top rated books found.";
        }
    }

}