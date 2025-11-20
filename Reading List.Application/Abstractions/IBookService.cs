using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Abstractions
{
    public interface IBookService : IService<BookDto>
    {
        Task<IEnumerable<Result<BookDto>>> GetTopRatedBooks(int count);

        Task<IEnumerable<Result<BookDto>>> GetFinishedBooks();

        Task<IEnumerable<Result<BookDto>>> GetBooksByAuthor(string author);

        Task<Result<BookDto>> MarkAsFinished(int bookId);

        Task<Result<BookDto>> SetRating(int bookId, decimal rating);
        Task<Result<BooksStats>> GetStatsAsync();


    }
}
