using CsvHelper;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Models;
using Reading_List.Infrastructure.FileService.Helpers;
using System.Collections.Concurrent;

namespace Reading_List.Infrastructure.FileService
{
    public class BookImportHandler : IBookImportHandler
    {
        private readonly IRepository<Book> bookRepository;
        private readonly ILoggerService logger;

        public BookImportHandler(IRepository<Book> bookRepository, ILoggerService logger)
        {
            this.bookRepository = bookRepository;
            this.logger = logger;
        }

        public async Task<Result<int>> ImportAsync(IEnumerable<string> csvFiles, CancellationToken ct = default)
        {
            var allBooks = new ConcurrentDictionary<int, Book>();
            int importedCount = 0;
            int duplicateCount = 0;
            int malformedCount = 0;
            int missingFiles = 0;

            var existingFiles = new List<string>();
            foreach (var csvFile in csvFiles)
            {
                if (File.Exists(csvFile))
                    existingFiles.Add(csvFile);
                else
                {
                    await logger.LogErrorAsync($"File not found: {csvFile}", null, ct);
                    missingFiles++;
                }
            }

            if (!existingFiles.Any())
            {
                await logger.LogInfoAsync("No valid files found for import.", ct);
                return Result<int>.Success(0);
            }

            var tasks = existingFiles.Select(file => Task.Run(async () =>
            {
                var (imported, duplicates, malformed) =
                    await BookImportHelper.ProcessFileAsync(file, allBooks, logger, ct);

                Interlocked.Add(ref importedCount, imported);
                Interlocked.Add(ref duplicateCount, duplicates);
                Interlocked.Add(ref malformedCount, malformed);
            }, ct));

            await Task.WhenAll(tasks);

            int added = 0;
            foreach (var book in allBooks.Values)
            {
                var result = await bookRepository.AddAsync(book);
                if (result.IsSuccess)
                    added++;
            }

            await logger.LogInfoAsync(
                $"Import summary: files={csvFiles.Count()}, missing={missingFiles}, imported={importedCount}, duplicates={duplicateCount}, malformed={malformedCount}, added={added}.",
                ct);

            return Result<int>.Success(added);
        }
    }
}
