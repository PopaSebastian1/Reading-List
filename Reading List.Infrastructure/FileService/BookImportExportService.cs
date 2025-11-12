using CsvHelper;
using CsvHelper.Configuration;
using Reading_List.Application.Abstractions;
using Reading_List.Application.ErrorHandlers;
using Reading_List.Domain.Enums;
using Reading_List.Domain.Models;
using System.Collections.Concurrent;
using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;

namespace Reading_List.Infrastructure.FileService
{
    public class BookImportExportService : IBookImportExportService
    {
        private readonly IRepository<Book> _bookRepository;

        public BookImportExportService(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<int>> ImportAsync(IEnumerable<string> csvFiles, CancellationToken ct = default)
        {
            var allBooks = new ConcurrentBag<Book>();
            // Read all files in parallel
            var tasks = csvFiles.Select(file => Task.Run(async () =>
            {
                if (!File.Exists(file))
                {
                    Console.WriteLine($"⚠️ File not found: {file}");
                    return;
                }

                var config = CSVHelperConfig.Default;
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, config);

                var books = csv.GetRecords<Book>().ToList();

                foreach (var book in books)
                    allBooks.Add(book);
            }, ct));

            await Task.WhenAll(tasks);

            var uniqueBooks = allBooks.GroupBy(b => b.Id).Select(g => g.First());

            int count = 0;
            foreach (var book in uniqueBooks)
            {
                var result = await _bookRepository.AddAsync(book);
                if (result.IsSuccess)
                    count++;
            }

            return Result<int>.Success(count);
        }

        public async Task<Result<bool>> ExportAsync(string filePath, CancellationToken ct = default)
        {
            var allBooks = await _bookRepository.GetAllAsync();
            var validBooks = allBooks
                .Where(r => r.IsSuccess && r.Value is not null)
                .Select(r => r.Value!)
                .ToList();

            var json = JsonSerializer.Serialize(validBooks, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json, ct);

            return Result<bool>.Success(true);
        }
    }
}
