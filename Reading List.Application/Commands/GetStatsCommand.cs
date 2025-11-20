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

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var statsResult = await _bookService.GetStatsAsync();
            if (!statsResult.IsSuccess)
            {
                return $"Error retrieving stats: {statsResult.ErrorMessage}";
                throw statsResult.Exception ?? new Exception("Unknown error retrieving stats.");
            }

            string pagesByGenre=statsResult.Value!.PagesByGenre.Any()?
                string.Join(Environment.NewLine, statsResult.Value.PagesByGenre.Select(kvp => $"  - {kvp.Key}: {kvp.Value} pages"))
                : "  N/A";

            var stats = statsResult.Value;
            return $"""
            
            === Reading List Stats ===
            Total books   : {stats.TotalBooks}
            Finished      : {stats.FinishedCount}
            Average rating: {(stats.AverageRating.HasValue ? stats.AverageRating.Value.ToString("0.00") : "N/A")}
            Pages by genre:
            {pagesByGenre}
            """;
            
        }
    }
}
