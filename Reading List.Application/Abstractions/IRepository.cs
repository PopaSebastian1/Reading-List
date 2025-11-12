using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;

namespace Reading_List.Application.Abstractions
{
    public interface IRepository<TKey, T>
        where TKey : notnull
        where T : class, IEntity<TKey>
    {
        Task<Result<T>> AddAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<bool>> DeleteAsync(T entity);
        Task<Result<T?>> GetByIdAsync(TKey id);
        Task<IEnumerable<Result<T>>> GetAllAsync();
    }
}