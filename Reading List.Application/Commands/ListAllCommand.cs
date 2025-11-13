using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class ListAllCommand : ICommand
    {

        public string Key => "2";
        public string Description => "List All Books";
        
        private readonly IBookService bookService;

        public ListAllCommand(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var books = await bookService.GetAllAsync();
            if (books.Count() == 0)
            {
                Console.WriteLine("No books found in the reading list.");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine(book.Value);
            }
        }
    }
}
