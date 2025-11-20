using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;

namespace Reading_List.Application.Abstractions
{
    public interface IService<T>
        where T : class, IEntity
    {
        Task<Result<T>> AddAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<bool>> DeleteAsync(T entity);
        Task<IEnumerable<Result<T>>> GetAllAsync();

        Task<Result<T>> GetByIdAsync(int id);

    }
}