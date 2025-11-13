using Reading_List.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Commands
{
    public class GetBooksByAuthor : ICommand
    {
        public string Key => "5";

        public string Description => "Get Books by Author";

        private readonly IBookService _bookService;

        public GetBooksByAuthor(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            Console.Write("Enter author name: ");
            var author = Console.ReadLine();

            var booksResult = await _bookService.GetBooksByAuthor(author ?? string.Empty, ct);

            foreach (var result in booksResult)
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
