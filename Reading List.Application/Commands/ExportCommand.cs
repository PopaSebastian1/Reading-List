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

        private readonly IBookExportService _exportService;

        public ExportBooksCommand(IBookExportService exportService)
        {
            _exportService = exportService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");
            Console.Write("Enter export file path (e.g., books.json or books.csv): ");
            var path= dataDir+ Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine(ErrorHandler.GenericError<bool>("Path cannot be empty").ErrorMessage);
                return;
            }

            var result = await _exportService.ExportAsync(path, ct);
            Console.WriteLine(result.IsSuccess
                ? $"Export succeeded -> {path}"
                : $"Export failed: {result.ErrorMessage}");
        }
    }
}
