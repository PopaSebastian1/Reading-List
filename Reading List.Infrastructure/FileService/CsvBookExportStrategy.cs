using CsvHelper;
using CsvHelper.Configuration;
using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.FileService
{
    public sealed class CsvBookExportStrategy : IBookExportStrategy
    {
        public IEnumerable<string> SupportedExtensions => new[] { ".csv" };

        public async Task<Result<bool>> ExportAsync(IEnumerable<Book> books, string filePath, CancellationToken ct = default)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };
            await using var writer = new StreamWriter(filePath, false);
            await using var csv = new CsvWriter(writer, config);
            await csv.WriteRecordsAsync(books, ct);
            return Result<bool>.Success(true);
        }
    }
}
