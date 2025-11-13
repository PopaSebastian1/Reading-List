using Reading_List.Domain.Models;

namespace Reading_List.Application.Abstractions
{
    public interface IBookService : IService<Book, int>
    {
        Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count);

        Task<IEnumerable<Result<Book>>> GetFinishedBooks();


    }
}
