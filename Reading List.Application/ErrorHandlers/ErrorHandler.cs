using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.ErrorHandler
{
    public static class ErrorHandler
    {
        public static Result<TOut> EntityNull<TOut>() =>
            Result<TOut>.Failure("Entity cannot be null");

        public static Result<TOut> EntityNotFound<TOut, TKey>(TOut entity, TKey id) =>
            Result<TOut>.Failure($"Entity with Id '{id}' was not found", entity);

        public static Result<TOut> EntityAlreadyExists<TOut>(TOut entity) =>
            Result<TOut>.Failure("Entity with the same Id already exists", entity);

        public static Result<TOut> EntityUpdateFailed<TOut>(TOut entity) =>
            Result<TOut>.Failure("Failed to update entity", entity);
    }
}
