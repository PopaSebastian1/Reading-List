using Reading_List.Application.Abstractions;

namespace Reading_List.Application.Commands
{
    public class GetFinishedCommand : ICommand
    {
        public string Key => "4";
        public string Description => "Get Finished Books";

        private readonly IBookService _bookService;

        public GetFinishedCommand(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var finishedResults = await _bookService.GetFinishedBooks();

            var finishedBooks = finishedResults
                .Where(r => r.IsSuccess && r.Value is not null)
                .Select(r => r.Value!)
                .ToList();

            if (!finishedBooks.Any())
            {
                Console.WriteLine("No finished books found.");
                return;
            }

            Console.WriteLine("Finished Books:");
            foreach (var book in finishedBooks)
            {
                Console.WriteLine($"{book.ToString()}");
            }
        }
    }
}
