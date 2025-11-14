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

        public async Task<Result<Book>> AddAsync(Book entity)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<Book>();

            var validation = await _bookValidator.ValidateAsync(entity);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return ErrorHandler.GenericError<Book>(message);
            }

            return await _bookRepository.AddAsync(entity);
        }

        public async Task<Result<Book>> UpdateAsync(Book entity)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<Book>();

            var validation = await _bookValidator.ValidateAsync(entity);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return ErrorHandler.GenericError<Book>(message);
            }

            return await _bookRepository.UpdateAsync(entity);
        }

        public async Task<Result<bool>> DeleteAsync(Book entity)
        {
            if (entity is null)
                return ErrorHandler.EntityNull<bool>();

            return await _bookRepository.DeleteAsync(entity);
        }

        public async Task<Result<Book>> GetByIdAsync(int id)
        {
            var result = await _bookRepository.GetByIdAsync(id);

            if (!result.IsSuccess || result.Value is null)
            {
                return ErrorHandler.EntityNotFound<Book, int>(id);
            }

            return result;
        }


        public async Task<IEnumerable<Result<Book>>> GetAllAsync()
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
        public async Task<IEnumerable<Result<Book>>> GetBooksByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return Enumerable.Empty<Result<Book>>();

            var allBooks = await _bookRepository.GetAllAsync();
            var booksByAuthor = allBooks
                .Where(r => r.IsSuccess && r.Value != null && r.Value.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return booksByAuthor;
        }
        public async Task<Result<Book>> MarkAsFinished(int bookId)
        {
            var book = _bookRepository.GetByIdAsync(bookId).Result.Value;
            if (book == null)
            {
                return ErrorHandler.EntityNotFound<Book, int>(bookId);
            }

            book.Finished = true;
            var updateResult = await UpdateAsync(book);
            return updateResult;
        }
        public async Task<Result<Book>> SetRating(int bookId, decimal rating)
        {
            if (rating < 1.00m || rating > 5.00m)
                return ErrorHandler.GenericError<Book>("Rating must be between 1 and 5.");

            var getResult = await _bookRepository.GetByIdAsync(bookId);
            if (!getResult.IsSuccess || getResult.Value is null)
                return ErrorHandler.EntityNotFound<Book, int>(bookId);

            var book = getResult.Value;
            book.Finished = true;
            book.Rating = rating;

            return await UpdateAsync(book);
        }
        public async Task<Result<BooksStats>> GetStatsAsync()
        {
            var results = await _bookRepository.GetAllAsync();

            var books = results
                .Where(r => r.IsSuccess && r.Value is not null)
                .Select(r => r.Value!)
                .ToList();

            var total = books.Count;
            var finished = books.Count(b => b.Finished);

            var ratings = books.Where(b => b.Rating.HasValue).Select(b => b.Rating!.Value).ToList();
            decimal? avgRating = ratings.Count > 0 ? Math.Round(ratings.Average(), 2) : null;

            var pagesByGenre = books
                .GroupBy(b => b.Genre)
                .ToDictionary(g => g.Key, g => g.Sum(b => b.Pages));

            var stats = new BooksStats(total, finished, avgRating, pagesByGenre);
            return Result<BooksStats>.Success(stats);
        }
    }
}
