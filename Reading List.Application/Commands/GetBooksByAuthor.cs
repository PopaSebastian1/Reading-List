using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;
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

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var author = ConsoleInputHandler.ReadNonEmptyString("Enter author name: ");

            var booksResult = await _bookService.GetBooksByAuthor(author ?? string.Empty);

            var books = new List<BookDto>();

            foreach (var result in booksResult)
            {
                if (result.IsSuccess && result.Value != null)
                {
                    var book = result.Value;
                    books.Add(book);
                }
                else
                {
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }
            }
            return books.Count > 0
                ? $"Books by {author}:\n" + string.Join("\n", books.Select(b => b.ToString()))
                : $"No books found by author: {author}";
        }
    }
}
