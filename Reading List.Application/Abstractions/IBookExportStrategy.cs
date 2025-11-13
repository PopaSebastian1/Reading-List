using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Abstractions
{
    public interface IBookExportStrategy
    {
        IEnumerable<string> SupportedExtensions { get; }
        Task<Result<bool>> ExportAsync(IEnumerable<Book> books, string filePath, CancellationToken ct = default);
    }
}
