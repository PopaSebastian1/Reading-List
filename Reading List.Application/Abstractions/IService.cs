using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;

namespace Reading_List.Application.Abstractions
{
    public interface IService<T, TKey>
        where TKey : notnull
        where T : class, IEntity<TKey>
    {
        Task<Result<T>> AddAsync(T entity, CancellationToken ct = default);
        Task<Result<T>> UpdateAsync(T entity, CancellationToken ct = default);
        Task<Result<bool>> DeleteAsync(T entity, CancellationToken ct = default);
        Task<IEnumerable<Result<T>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<T?>> GetByIdAsync(TKey id, CancellationToken ct = default);
    }
}