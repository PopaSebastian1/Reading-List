using FluentValidation;
using Reading_List.Application.Abstractions;
using Reading_List.Application.ErrorHandlers;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Services
{
    public sealed class BookService : IBookService
    {
        private readonly IRepository<int, Book> _bookRepository;
        private readonly IValidator<Book> _bookValidator;

        public BookService(IRepository<int, Book> bookRepository, IValidator<Book> bookValidator)
        {
            _bookRepository = bookRepository;
            _bookValidator = bookValidator;
        }

        public Task<Result<Book>> AddAsync(Book entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteAsync(Book entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Result<Book>>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Book?>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Result<Book>>> GetFinishedBooks()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Result<Book>>> GetTopRatedBooks(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Book>> UpdateAsync(Book entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}