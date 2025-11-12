using Reading_List.Domain.Models;
namespace Reading_List.Application.Abstractions
{
    public interface IRepository<T> where T : class
    {

        Task<Result<T>> AddAsync(T entity);

        Task<Result<T>> UpdateAsync(T entity);

        Task<Result<bool>> DeleteAsync(T entity);

        Task<Result<T?>> GetByIdAsync(Guid id);

        Task<IEnumerable<Result<T>>> GetAllAsync();

    }
}
