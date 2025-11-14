using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
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
            var author = ConsoleInputHandler.ReadNonEmptyString("Enter author name: ");

            var booksResult = await _bookService.GetBooksByAuthor(author ?? string.Empty);

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
