using Reading_List.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Models
{
    public class BooksStats
    {
        public int TotalBooks { get; }
        public int FinishedCount { get; }
        public decimal? AverageRating { get; }
        public IReadOnlyDictionary<Genre, int> PagesByGenre { get; }

        public BooksStats(int totalBooks, int finishedCount, decimal? averageRating, IDictionary<Genre, int> pagesByGenre)
        {
            TotalBooks = totalBooks;
            FinishedCount = finishedCount;
            AverageRating = averageRating;
            PagesByGenre = new Dictionary<Genre, int>(pagesByGenre);
        }
    }
}
