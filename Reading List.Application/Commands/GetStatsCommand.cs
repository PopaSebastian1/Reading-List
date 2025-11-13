using Reading_List.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Commands
{
    public class GetStatsCommand : ICommand
    {
        public string Key => "9";

        public string Description => "Show statistics (total, finished, avg rating, pages by genre)";

        private readonly IBookService _bookService;

        public GetStatsCommand(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var statsResult = await _bookService.GetStatsAsync();
            if (!statsResult.IsSuccess)
            {
                Console.WriteLine($"Error retrieving stats: {statsResult.ErrorMessage}");
                return;
            }

            var stats = statsResult.Value;
            Console.WriteLine();
            Console.WriteLine("=== Reading List Stats ===");
            Console.WriteLine($"Total books   : {stats!.TotalBooks}");
            Console.WriteLine($"Finished      : {stats.FinishedCount}");
            Console.WriteLine($"Average rating: {(stats.AverageRating.HasValue ? stats.AverageRating.Value.ToString("0.00") : "N/A")}");
            Console.WriteLine("Pages by genre:");
            foreach (var kv in stats.PagesByGenre.OrderBy(k => k.Key.ToString()))
            {
                Console.WriteLine($" - {kv.Key}: {kv.Value}");

            }
        }
    }
}
