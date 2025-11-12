using Reading_List.Application.Abstractions;
using Reading_List.Application.ErrorHandler;
using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.Repositories
{
    public class InMemoryRepository<TKey, T> : IRepository<T>
        where TKey : notnull
        where T : class, IEntity<TKey>
        
    {
        private readonly ConcurrentDictionary<TKey, T> store;
        private readonly Func<T, TKey> keySelector;

      public InMemoryRepository(Func<T, TKey> keySelector)
        {
            store = new ConcurrentDictionary<TKey, T>();
            this.keySelector = keySelector;
        }

        public Task<Result<T>> AddAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            var key = keySelector(entity);
            if (!store.TryAdd(key, entity))
                return Task.FromResult(ErrorHandler.EntityAlreadyExists(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<T>> UpdateAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            var key = keySelector(entity);

            if (!store.TryGetValue(key, out var current))
                return Task.FromResult(ErrorHandler.EntityNotFound<T, TKey>(entity, key));

            if (!store.TryUpdate(key, entity, current))
                return Task.FromResult(ErrorHandler.EntityUpdateFailed(entity));

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<T>> DeleteAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(ErrorHandler.EntityNull<T>());

            var key = keySelector(entity);
            var removed = store.TryRemove(key, out _);

            return Task.FromResult(removed
                ? ErrorHandler.EntityUpdateFailed(entity)
                : ErrorHandler.EntityNotFound(entity, key));
        }


        public Task<IEnumerable<Result<T>>> GetAllAsync()
        {
            var results = store.Values.Select(Result<T>.Success);
            return Task.FromResult(results);
        }

        public Task<Result<T?>> GetByIdAsync(TKey id)
        {
            if (store.TryGetValue(id, out var entity))
                return Task.FromResult(Result<T?>.Success(entity));

            return Task.FromResult(ErrorHandler.EntityNotFound<T?, TKey>(entity, id));
        }
    }
}
