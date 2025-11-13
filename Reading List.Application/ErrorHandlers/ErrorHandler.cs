using Reading_List.Domain.Models;

namespace Reading_List.Application.ErrorHandlers
{
    public static class ErrorHandler
    {
        public static Result<TOut> EntityNull<TOut>() =>
            Result<TOut>.Failure("Entity cannot be null");

        public static Result<TOut> EntityNotFound<TOut, TKey>(TKey id) =>
            Result<TOut>.Failure($"Entity with Id '{id}' was not found");

        public static Result<TOut> EntityAlreadyExists<TOut>(TOut entity) =>
            Result<TOut>.Failure("Entity with the same Id already exists", entity);

        public static Result<TOut> EntityUpdateFailed<TOut>(TOut entity) =>
            Result<TOut>.Failure("Failed to update entity", entity);

        public static Result<TOut> FileNotFound<TOut>(string fileName) =>
            Result<TOut>.Failure($"File '{fileName}' was not found");


        public static Result<TOut> GenericError<TOut>(string message) =>
            Result<TOut>.Failure(message);


    }
}
