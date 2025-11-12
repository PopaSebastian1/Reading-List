using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;
using System.Collections.Concurrent;
using Reading_List.Application.ErrorHandlers;

namespace Reading_List.Infrastructure.Repositories
{
    public class InMemoryRepository<TKey, T> :
        IRepository<T>
        where TKey : notnull
        where T : class
    {
        private readonly ConcurrentDictionary<TKey, T> _store = new();
        private readonly Func<T, TKey> _keySelector;

        public InMemoryRepository(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        public Task<Result<T>> AddAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            var key = _keySelector(entity);

            if (!_store.TryAdd(key, entity))
                return Task.FromResult(ErrorHandler.EntityAlreadyExists<T>(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<T>> UpdateAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            var key = _keySelector(entity);

            if (!_store.TryGetValue(key, out var current))
                return Task.FromResult(ErrorHandler.EntityNotFound<T, TKey>(key));

            if (!_store.TryUpdate(key, entity, current))
                return Task.FromResult(ErrorHandler.EntityUpdateFailed<T>(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<bool>> DeleteAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<bool>());

            var key = _keySelector(entity);

            var removed = _store.TryRemove(key, out _);
            return Task.FromResult(
                removed
                    ? Result<bool>.Success(true)
                    : ErrorHandler.EntityNotFound<bool, TKey>(key));
        }

        public Task<IEnumerable<Result<T>>> GetAllAsync()
        {
            var results = _store.Values.Select(Result<T>.Success);
            return Task.FromResult(results);
        }
    }
}
