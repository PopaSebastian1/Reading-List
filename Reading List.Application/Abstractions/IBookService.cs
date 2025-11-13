using Reading_List.Domain.Models;

namespace Reading_List.Application.Abstractions
{
    public interface IBookService : IService<Book>
    {
        Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count, CancellationToken ct = default);

        Task<IEnumerable<Result<Book>>> GetFinishedBooks(CancellationToken ct = default);

        Task<IEnumerable<Result<Book>>> GetBooksByAuthor(string author, CancellationToken ct = default);

        Task<Result<Book>> MarkAsFinished(int bookId, CancellationToken ct = default);

        Task<Result<Book>> SetRating(int bookId, decimal rating, CancellationToken ct = default);
        Task<Result<BooksStats>> GetStatsAsync(CancellationToken ct = default);


    }
}
