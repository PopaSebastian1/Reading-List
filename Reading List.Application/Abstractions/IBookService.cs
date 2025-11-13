using Reading_List.Domain.Models;

namespace Reading_List.Application.Abstractions
{
    public interface IBookService : IService<Book>
    {
        Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count);

        Task<IEnumerable<Result<Book>>> GetFinishedBooks();

        Task<IEnumerable<Result<Book>>> GetBooksByAuthor(string author);

        Task<Result<Book>> MarkAsFinished(int bookId);

        Task<Result<Book>> SetRating(int bookId, decimal rating);
        Task<Result<BooksStats>> GetStatsAsync();


    }
}
