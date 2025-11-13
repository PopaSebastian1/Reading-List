using FluentValidation;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
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

        public async Task<Result<Book>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var result = await _bookRepository.GetByIdAsync(id);

            if (!result.IsSuccess || result.Value is null)
            {
                return ErrorHandler.EntityNotFound<Book, int>(id);
            }

            return result;
        }


        public async Task<IEnumerable<Result<Book>>> GetAllAsync(CancellationToken ct = default)
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Result<Book>>> GetFinishedBooks(CancellationToken ct = default)
        {
            var allBooks = await _bookRepository.GetAllAsync();
            return allBooks.Where(r => r.IsSuccess && r.Value?.Finished == true);
        }
        public async Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count, CancellationToken ct = default)
        {
            if (count <= 0) count = 5;

            var allBooks = await _bookRepository.GetAllAsync();

            var topRatedBooks = allBooks
                .Where(r => r.IsSuccess && r.Value?.Rating.HasValue == true)
                .Select(r => r.Value!)
                .OrderByDescending(b => b.Rating)
                .Take(count)
                .Select(Result<Book>.Success)
                .ToList();

            return topRatedBooks;
        }
        public async Task<IEnumerable<Result<Book>>> GetBooksByAuthor(string author, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(author))
                return Enumerable.Empty<Result<Book>>();

            var allBooks = await _bookRepository.GetAllAsync();
            var booksByAuthor = allBooks
                .Where(r => r.IsSuccess && r.Value != null && r.Value.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return booksByAuthor;
        }
        public async Task<Result<Book>> MarkAsFinished(int bookId, CancellationToken ct = default)
        {
            var book = _bookRepository.GetByIdAsync(bookId).Result.Value;
            if (book == null)
            {
                return ErrorHandler.EntityNotFound<Book, int>(bookId);
            }

            book.Finished = true;
            var updateResult = await UpdateAsync(book, ct);
            return updateResult;
        }
        public async Task<Result<Book>> SetRating(int bookId, decimal rating, CancellationToken ct = default)
        {
            if (rating < 1.00m || rating > 5.00m)
                return ErrorHandler.GenericError<Book>("Rating must be between 1 and 5.");

            var getResult = await _bookRepository.GetByIdAsync(bookId);
            if (!getResult.IsSuccess || getResult.Value is null)
                return ErrorHandler.EntityNotFound<Book, int>(bookId);

            var book = getResult.Value;
            book.Finished = true;
            book.Rating = rating;

            return await UpdateAsync(book, ct);
        }

    }
}
