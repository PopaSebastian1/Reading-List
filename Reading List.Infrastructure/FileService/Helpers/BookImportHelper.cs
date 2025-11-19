using CsvHelper;
using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.FileService.Helpers
{
    public static class BookImportHelper
    {

        public static async Task<(int imported, int duplicates, int malformed)> ProcessFileAsync(
            string file,
            ConcurrentDictionary<int, Book> allBooks,
            ILoggerService logger,
            CancellationToken ct = default)
        {
            int importedCount = 0;
            int duplicateCount = 0;
            int malformedCount = 0;

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
                            malformedCount++;
                            await logger.LogErrorAsync(
                                $"Invalid row in file {file}. Skipped {csv.Parser.RawRecord ?? "<unknown>"}",
                                null, ct);
                            continue;
                        }

                        if (allBooks.TryAdd(book.Id, book))
                        {
                            importedCount++;
                        }
                        else
                        {
                            duplicateCount++;
                            await logger.LogWarningAsync(
                                $"Duplicate book with ID {book.Id} in file {file}. Skipped (first arrival wins).",
                                ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        malformedCount++;
                        await logger.LogErrorAsync(
                            $"Invalid row in file {file}. Skipped.\nRaw CSV: {csv.Parser.RawRecord ?? "<unknown>"}",
                            ex,
                            ct);
                    }
                }
            }
            catch (Exception ex)
            {
                await logger.LogErrorAsync($"Failed to read CSV file {file}.", ex, ct);
            }

            return (importedCount, duplicateCount, malformedCount);
        }
    }
}
