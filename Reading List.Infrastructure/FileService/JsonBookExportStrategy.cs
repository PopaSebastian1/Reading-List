using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reading_List.Infrastructure.FileService
{
    public sealed class JsonBookExportStrategy : IBookExportStrategy
    {
        public string SupportedExtensions => ".json";

        public async Task<Result<bool>> ExportAsync(IEnumerable<Book> books, string filePath, CancellationToken ct = default)
        {
            var list = books.ToList();
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json, ct);
            return Result<bool>.Success(true);
        }
    }
}
