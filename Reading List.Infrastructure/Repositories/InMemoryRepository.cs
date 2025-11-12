using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using Reading_List.Domain.Models.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.Repositories
{
    public class InMemoryRepository<TKey, T> :
        IRepository<T>
        where T : class, IEntity<TKey>
    {
        private readonly ConcurrentDictionary<TKey, T> store = new();


        public Task<Result<T>> AddAsync(T entity)
        {
            if (entity is null)
            {
                return Task.FromResult(Result<T>.Failure("Entity cannot be null", entity));
            }

            if (!store.TryAdd(entity.Id, entity))
            {
                return Task.FromResult(Result<T>.Failure("Entity with the same Id already exists", entity));
            }

            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<T>> UpdateAsync(T entity)
        {
            if (entity is null)
                return Task.FromResult(Result<T>.Failure("Entity cannot be null", entity));

            if (!store.ContainsKey(entity.Id))
                return Task.FromResult(Result<T>.Failure($"Entity with Id '{entity.Id}' was not found", entity));

            if(!store.TryUpdate(entity.Id, entity, store[entity.Id]))
            {
                return Task.FromResult(Result<T>.Failure("Failed to update entity", entity));
            }


            return Task.FromResult(Result<T>.Success(entity));
        }

        public Task<Result<bool>> DeleteAsync(T entity)
        {
            if (entity is null)
            {
                return Task.FromResult(Result<bool>.Failure("Entity cannot be null", false));
            }
          
            var removed= store.TryRemove(entity.Id, out _);

            return Task.FromResult(removed
                ? Result<bool>.Success(true)
                : Result<bool>.Failure($"Entity with Id '{entity.Id}' was not found", false));
        }


        public Task<IEnumerable<Result<T>>> GetAllAsync()
        {
            var results = store.Values.Select(entity => Result<T>.Success(entity));
            return Task.FromResult(results);
        }

        public Task<Result<T?>> GetByIdAsync(TKey id)
        {
            if (store.TryGetValue(id, out var entity))
                return Task.FromResult(Result<T?>.Success(entity));

            return Task.FromResult(Result<T?>.Failure($"Entity with Id '{id}' not found", default));
        }
    }

}