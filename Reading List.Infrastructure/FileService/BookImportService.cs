using CsvHelper;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Models;
using System.Collections.Concurrent;

namespace Reading_List.Infrastructure.FileService
{
    public class BookImportService : IBookImportService
    {
        private readonly IRepository<Book> bookRepository;
        private readonly ILoggerService logger;

        public BookImportService(IRepository<Book> bookRepository, ILoggerService logger)
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
                await logger.LogInfoAsync($"No valid files found for import.", ct);
                return Result<int>.Success(0);
            }

            var tasks = existingFiles.Select(file => Task.Run(async () =>
            {
                var config = CSVHelperConfig.Default;
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, config);

                try
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        if (ct.IsCancellationRequested)
                            break;

                        try
                        {
                            var book = csv.GetRecord<Book>();
                            if (book is null)
                            {
                                Interlocked.Increment(ref malformedCount);
                                await logger.LogErrorAsync(
                                    $"Invalid row in file {file}. Skipped {csv.Parser.RawRecord ?? "<unknown>"}",
                                    null, ct);
                                continue;
                            }

                            if (allBooks.TryAdd(book.Id, book))
                            {
                                Interlocked.Increment(ref importedCount);
                            }
                            else
                            {
                                Interlocked.Increment(ref duplicateCount);
                                await logger.LogWarningAsync(
                                    $"Duplicate book with ID {book.Id} in file {file}. Skipped (first arrival wins).",
                                    ct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref malformedCount);
                            await logger.LogErrorAsync(
                                $"Invalid row in file {file}. Skipped.\nRaw CSV: {csv.Parser.RawRecord ?? "<unknown>"}",null
                                , ct);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await logger.LogErrorAsync($"Failed to read CSV file {file}.", ex, ct);
                }
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
