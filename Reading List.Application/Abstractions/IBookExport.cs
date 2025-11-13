using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Abstractions
{
    public interface IBookExportService
    {
        Task<Result<bool>> ExportAsync(string filePath, CancellationToken ct = default);
    }
}
