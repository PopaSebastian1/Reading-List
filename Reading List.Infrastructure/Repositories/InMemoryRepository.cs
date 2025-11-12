using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;
using System.Collections.Concurrent;
using Reading_List.Application.ErrorHandlers;
namespace Reading_List.Infrastructure.Repositories
{
    public class InMemoryRepository<TKey, T> :
        IRepository<TKey, T>
        where TKey : notnull
        where T : class, IEntity<TKey>
    {
        private readonly ConcurrentDictionary<TKey, T> _store = new();

        public Task<Result<T>> AddAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            if (!_store.TryAdd(entity.Id, entity))
                return Task.FromResult(ErrorHandler.EntityAlreadyExists<T>(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<T>> UpdateAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            if (!_store.TryGetValue(entity.Id, out var current))
                return Task.FromResult(ErrorHandler.EntityNotFound<T, TKey>(entity.Id));

            if (!_store.TryUpdate(entity.Id, entity, current))
                return Task.FromResult(ErrorHandler.EntityUpdateFailed<T>(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<bool>> DeleteAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<bool>());

            var removed = _store.TryRemove(entity.Id, out _);
            return Task.FromResult(
                removed
                    ? Result<bool>.Success(true)
                    : ErrorHandler.EntityNotFound<bool, TKey>(entity.Id));
        }

        public Task<Result<T?>> GetByIdAsync(TKey id)
        {
            return Task.FromResult(
                _store.TryGetValue(id, out var entity)
                    ? Result<T?>.Success(entity)
                    : ErrorHandler.EntityNotFound<T?, TKey>(id));
        }

        public Task<IEnumerable<Result<T>>> GetAllAsync()
        {
            var results = _store.Values.Select(Result<T>.Success);
            return Task.FromResult(results);
        }
    }
}