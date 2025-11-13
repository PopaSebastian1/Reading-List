using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;

namespace Reading_List.Application.Abstractions
{
    public interface IService<T>
        where T : class, IEntity
    {
        Task<Result<T>> AddAsync(T entity, CancellationToken ct = default);
        Task<Result<T>> UpdateAsync(T entity, CancellationToken ct = default);
        Task<Result<bool>> DeleteAsync(T entity, CancellationToken ct = default);
        Task<IEnumerable<Result<T>>> GetAllAsync(CancellationToken ct = default);

        Task<Result<T>> GetByIdAsync(int id, CancellationToken ct = default);

    }
}