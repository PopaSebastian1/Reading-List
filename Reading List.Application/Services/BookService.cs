using FluentValidation;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Services
{
    public sealed class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IValidator<BookDto> _bookValidator;
        private readonly IMapper<Book, BookDto> _mapper;

        public BookService(IRepository<Book> bookRepository, IValidator<BookDto> bookValidator, IMapper<Book, BookDto> mapper)
        {
            _bookRepository = bookRepository;
            _bookValidator = bookValidator;
            _mapper = mapper;
        }

        public async Task<Result<BookDto>> AddAsync(BookDto dto)
        {
            if (dto is null)
                return ErrorHandler.EntityNull<BookDto>();

            var validation = await _bookValidator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return Result<BookDto>.Failure(message);
            }

            var entity = _mapper.toEntity(dto);
            var addResult = await _bookRepository.AddAsync(entity);

            return addResult.IsSuccess && addResult.Value is not null
                ? Result<BookDto>.Success(_mapper.toDto(addResult.Value))
                : Result<BookDto>.Failure(addResult.ErrorMessage ?? "Failed to add book.");
        }

        public async Task<Result<BookDto>> UpdateAsync(BookDto dto)
        {
            if (dto is null)
                return ErrorHandler.EntityNull<BookDto>();

            var validation = await _bookValidator.ValidateAsync(dto);
            var entity = _mapper.toEntity(dto);
            if (!validation.IsValid)
            {
                var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return ErrorHandler.GenericError<BookDto>(message);
            }

            var updateResult = await _bookRepository.UpdateAsync(entity);
            return updateResult.IsSuccess && updateResult.Value is not null
                ? Result<BookDto>.Success(_mapper.toDto(updateResult.Value))
                : Result<BookDto>.Failure(updateResult.ErrorMessage ?? "Failed to update book.");
        }

        public async Task<Result<bool>> DeleteAsync(BookDto dto)
        {
            if (dto is null)
                return ErrorHandler.EntityNull<bool>();

            var entity = _mapper.toEntity(dto);
            return await _bookRepository.DeleteAsync(entity);
        }

        public async Task<Result<BookDto>> GetByIdAsync(int id)
        {
            var result = await _bookRepository.GetByIdAsync(id);

            if (!result.IsSuccess || result.Value is null)
            {
                return ErrorHandler.EntityNotFound<BookDto, int>(id);
            }

            return Result<BookDto>.Success(_mapper.toDto(result.Value));
        }


        public async Task<IEnumerable<Result<BookDto>>> GetAllAsync()
        {
            var all = await _bookRepository.GetAllAsync();
            return all.Select(r =>
                r.IsSuccess && r.Value is not null
                    ? Result<BookDto>.Success(_mapper.toDto(r.Value))
                    : Result<BookDto>.Failure(r.ErrorMessage ?? "Unknown error"));
        }

        public async Task<IEnumerable<Result<BookDto>>> GetFinishedBooks()
        {
            var allBooks = await _bookRepository.GetAllAsync();
            var bookDtos = new List<BookDto>();
            foreach (var book in allBooks)
            {
                bookDtos.Add(_mapper.toDto(book.Value!));
            }
            return bookDtos.Where(r => r.Finished)
                .Select(Result<BookDto>.Success)
                .ToList();
        }
        public async Task<IEnumerable<Result<BookDto>>> GetTopRatedBooks(int count)
        {
            if (count <= 0) count = 5;
            var all = await _bookRepository.GetAllAsync();

            return all
                .Where(r => r.IsSuccess && r.Value?.Rating.HasValue == true)
                .Select(r => r.Value!)
                .OrderByDescending(b => b.Rating)
                .Take(count)
                .Select(b => Result<BookDto>.Success(_mapper.toDto(b)))
                .ToList();
        }


        public async Task<IEnumerable<Result<BookDto>>> GetBooksByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return Enumerable.Empty<Result<BookDto>>();

            var all = await _bookRepository.GetAllAsync();
            return all
                .Where(r => r.IsSuccess &&
                            r.Value!.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                .Select(r => Result<BookDto>.Success(_mapper.toDto(r.Value!)));
        }

        public async Task<Result<BookDto>> MarkAsFinished(int bookId)
        {
            var getResult = await _bookRepository.GetByIdAsync(bookId);
            if (!getResult.IsSuccess || getResult.Value is null)
                return ErrorHandler.EntityNotFound<BookDto, int>(bookId);

            var book = getResult.Value;
            book.Finished = "yes";

            var updateResult = await _bookRepository.UpdateAsync(book);
            return updateResult.IsSuccess && updateResult.Value is not null
                ? Result<BookDto>.Success(_mapper.toDto(updateResult.Value))
                : Result<BookDto>.Failure(updateResult.ErrorMessage ?? "Failed to mark finished.");
        }

        public async Task<Result<BookDto>> SetRating(int bookId, decimal rating)
        {
            if (rating < 1m || rating > 5m)
                return Result<BookDto>.Failure("Rating must be between 1 and 5.");

            var getResult = await _bookRepository.GetByIdAsync(bookId);
            if (!getResult.IsSuccess || getResult.Value is null)
                return ErrorHandler.EntityNotFound<BookDto, int>(bookId);

            var book = getResult.Value;
            book.Finished = "yes";
            book.Rating = rating;

            var updateResult = await _bookRepository.UpdateAsync(book);
            return updateResult.IsSuccess && updateResult.Value is not null
                ? Result<BookDto>.Success(_mapper.toDto(updateResult.Value))
                : Result<BookDto>.Failure(updateResult.ErrorMessage ?? "Failed to set rating.");
        }
        public async Task<Result<BooksStats>> GetStatsAsync()
        {
            var results = await _bookRepository.GetAllAsync();

            var books = results
                .Where(r => r.IsSuccess && r.Value is not null)
                .Select(r => r.Value!)
                .ToList();

            var total = books.Count;
            var finished = books.Count(b => b.isFinished());

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
