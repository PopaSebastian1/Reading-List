using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Commands
{
    public sealed class ExportBooksCommand : ICommand
    {
        public string Key => "8";
        public string Description => "Export books to a file (.json or .csv)";

        private readonly IBookExportHandler _exportHandler;

        public ExportBooksCommand(IBookExportHandler exportService)
        {
            _exportHandler = exportService;
        }

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");
            var fileName = ConsoleInputHandler.ReadNonEmptyString("Enter export file name (e.g., books.json or books.csv): ");
            var path = Path.Combine(dataDir, fileName ?? string.Empty);

            var result = await _exportHandler.ExportAsync(path, ct);
            return result.IsSuccess
                ? $"Export succeeded -> {path}"
                : $"Export failed: {result.ErrorMessage}";
        }
    }
}
