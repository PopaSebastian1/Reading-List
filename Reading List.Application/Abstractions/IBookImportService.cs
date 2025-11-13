using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Abstractions
{
    public interface IBookImportService
    {
        Task<Result<int>> ImportAsync(IEnumerable<string> csvFiles, CancellationToken ct = default);
    }
}
