using FluentValidation;
using Reading_List.Application.Abstractions;
using Reading_List.Application.ErrorHandlers;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Services
{
    public sealed class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IValidator<Book> _bookValidator;

        public BookService(IRepository<Book> bookRepository, IValidator<Book> bookValidator)
        {
            _bookRepository = bookRepository;
            _bookValidator = bookValidator;
        }

        public async Task<Result<Book>> AddAsync(Book entity, CancellationToken ct = default)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<Book>();

            var validation = await _bookValidator.ValidateAsync(entity, ct);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return ErrorHandler.GenericError<Book>(message);
            }

            return await _bookRepository.AddAsync(entity);
        }

        public async Task<Result<Book>> UpdateAsync(Book entity, CancellationToken ct = default)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<Book>();

            var validation = await _bookValidator.ValidateAsync(entity, ct);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return ErrorHandler.GenericError<Book>(message);
            }

            return await _bookRepository.UpdateAsync(entity);
        }

        public async Task<Result<bool>> DeleteAsync(Book entity, CancellationToken ct = default)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<bool>();

            return await _bookRepository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<Result<Book>>> GetAllAsync(CancellationToken ct = default)
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Result<Book>>> GetFinishedBooks()
        {
            var allBooks = await _bookRepository.GetAllAsync();
            return allBooks.Where(r => r.IsSuccess && r.Value?.Finished == true);
        }

        public async Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count)
        {
            var allBooks = await _bookRepository.GetAllAsync();

            return allBooks
                .Where(r => r.IsSuccess && r.Value is not null)
                .OrderByDescending(r => r.Value!.Rating)
                .Take(count);
        }
    }
}
