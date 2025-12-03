using Reading_List.Application.Abstractions;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reading_List.CLI
{
    public sealed class WithoutCommands
    {
        private readonly IBookService _bookService;
        private readonly IBookImportHandler _importHandler;

        public WithoutCommands(IBookService bookService, IBookImportHandler importHandler)
        {
            _bookService = bookService;
            _importHandler = importHandler;
        }

        public async Task<string> ImportDefaultAsync(CancellationToken ct = default)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");
            var path = Path.Combine(dataDir, "books_part1.csv");

            var result = await _importHandler.ImportAsync(new[] { path }, ct);
            return result.IsSuccess
                ? $"Import succeeded -> {path}"
                : $"Import failed: {result.ErrorMessage ?? result.Exception?.Message ?? "Unknown error"}";
        }

        public async Task ListAllAsync(CancellationToken ct = default)
        {
            var results = await _bookService.GetAllAsync();
            PrintResultList("All Books", results);
        }

        public async Task ListTopRatedAsync(int count = 5, CancellationToken ct = default)
        {
            var results = await _bookService.GetTopRatedBooks(count);
            PrintResultList($"Top {count} Rated Books", results);
        }

        public async Task ListFinishedAsync(CancellationToken ct = default)
        {
            var results = await _bookService.GetFinishedBooks();
            PrintResultList("Finished Books", results);
        }

        public async Task ShowStatsAsync(CancellationToken ct = default)
        {
            var statsResult = await _bookService.GetStatsAsync();
            if (!statsResult.IsSuccess || statsResult.Value is null)
            {
                Console.WriteLine($"Stats unavailable: {statsResult.ErrorMessage ?? statsResult.Exception?.Message ?? "Unknown error"}");
                return;
            }

            var s = statsResult.Value;
            Console.WriteLine($"""
                === Stats ===
                Total Books     : {s.TotalBooks}
                Finished        : {s.FinishedCount}
                Average Rating  : {(s.AverageRating?.ToString("0.00") ?? "N/A")}
                Pages by Genre  :
                {string.Join(Environment.NewLine, s.PagesByGenre.Select(kv => $"  {kv.Key,-15} -> {kv.Value}"))}
                """);
        }

        private static void PrintResultList(string header, IEnumerable<Result<BookDto>> results)
        {
            var items = results.Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!).ToList();
            Console.WriteLine(items.Count == 0
                                ? $"""
                === {header} ===
                No items found.
                """
                                : $"""
                === {header} ===
                {string.Join(Environment.NewLine, items.Select(b => b.ToString()))}
                """);
                        }
    }
}