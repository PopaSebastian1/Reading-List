using CsvHelper;
using CsvHelper.Configuration;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Enums;
using Reading_List.Domain.Models;
using System.Collections.Concurrent;
using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;

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
            int missingFiles = 0;
            foreach (var csvFile in csvFiles)
            {
                if (!File.Exists(csvFile))
                {
                    var msg = ErrorHandler.FileNotFound<int>(csvFile).ErrorMessage!;
                    await logger.LogErrorAsync(msg, null, ct);
                    missingFiles++;
                }
            }

            var tasks = csvFiles.Select(file => Task.Run(async () =>
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
                        try
                        {
                            var book = csv.GetRecord<Book>();

                            if (book == null)
                            {
                                var raw = csv.Parser.RawRecord ?? "<unknown>";

                                await logger.LogErrorAsync(
                                    $"Invalid row (null record) in file {file}. Skipped.\nRaw CSV: {raw}",
                                    null,
                                    ct
                                );

                                continue;
                            }

                            if (!allBooks.TryAdd(book.Id, book))
                            {
                                await logger.LogWarningAsync(
                                    $"Duplicate book with ID {book.Id} in file {file}. Skipped.",
                                    ct
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            var raw = csv.Parser.RawRecord ?? "<unknown>";

                            await logger.LogErrorAsync(
                                $"Invalid row in file {file}. Skipped.\nRaw CSV: {raw}",
                                ex,
                                ct
                            );
                        }
                    }
                }
                catch (Exception ex) {
                    var msg = $"Failed to read CSV file {file}.";
                    await logger.LogErrorAsync(msg, ex, ct);}
                }, ct));

            await Task.WhenAll(tasks);

            int count = 0;
            foreach (var book in allBooks)
            {
                var result = await bookRepository.AddAsync(book.Value);
                if (result.IsSuccess)
                    count++;
            }
            await logger.LogInfoAsync($"Import summary: Files requested={csvFiles.Count()}, missing={missingFiles}, unique books parsed={allBooks.Count}, added={count}.", ct);


            return Result<int>.Success(count);
        }
    }
}
