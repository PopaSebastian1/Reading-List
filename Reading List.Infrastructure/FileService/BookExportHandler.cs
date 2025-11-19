using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.FileService
{
    public sealed class BookExportHandler : IBookExportHandler
    {
        private readonly IEnumerable<IBookExportStrategy> _strategies;
        private readonly IRepository<Book> _repository;

        public BookExportHandler(IEnumerable<IBookExportStrategy> strategies,
                                 IRepository<Book> repository,
                                 ILoggerService logger)
        {
            _strategies = strategies;
            _repository = repository;
        }

        public async Task<Result<bool>> ExportAsync(string filePath, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return ErrorHandler.GenericError<bool>("File path cannot be empty");

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            var strategy = _strategies.FirstOrDefault(s => s.SupportedExtensions.Contains(ext));
            if (strategy is null)
                return ErrorHandler.GenericError<bool>($"No export strategy found for extension '{ext}'");

            var allResults = await _repository.GetAllAsync();
            var books = allResults
                .Where(r => r.IsSuccess && r.Value != null)
                .Select(r => r.Value!)
                .ToList();

            if (!books.Any())
            {
                return ErrorHandler.GenericError<bool>("No books to export");
            }

            try
            {
                var result = await strategy.ExportAsync(books, filePath, ct);
                if (result.IsSuccess)
                    return result;
                else
                    return ErrorHandler.GenericError<bool>($"Export failed: {result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                return ErrorHandler.GenericError<bool>($"Export failed: {ex.Message}");
            }
        }
    }
}
