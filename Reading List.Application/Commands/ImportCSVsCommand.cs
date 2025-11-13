using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Commands
{
    public sealed class ImportCSVsCommand : ICommand
    {
        public string Key => "1";
        public string Description => "Import one or more CSV files (comma-separated). Enter for defaults.";

        private readonly IBookImportService _importExportService;

        public ImportCSVsCommand(IBookImportService bookImportExportService)
        {
            _importExportService = bookImportExportService;
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");

            Console.WriteLine("Enter CSV file paths separated by commas.");
            Console.Write("Paths: ");
            var input = Console.ReadLine();

            List<string> rawPaths= input
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();


            var resolved = new List<string>();
            foreach (var p in rawPaths)
            {
                string candidate = p;
                if (!Path.IsPathRooted(candidate))
                    candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, candidate));

                if (!File.Exists(candidate))
                {
                    var fileName = Path.GetFileName(p);
                    var fallback = Path.Combine(dataDir, fileName);
                    if (File.Exists(fallback))
                    {
                        candidate = fallback;
                    }
                }

                resolved.Add(candidate);
            }
            Console.WriteLine("Importing CSV files...");
            Result<int> importResult;
            try
            {
                importResult = await _importExportService.ImportAsync(resolved, ct);
            }
            catch (Exception ex)
            {
                importResult = ErrorHandler.GenericError<int>($"An error occurred during import: {ex.Message}");
            }

            Console.WriteLine(importResult.IsSuccess
                ? $"Successfully imported {importResult.Value} book(s)."
                : $"Import failed: {importResult.ErrorMessage}");
        }
    }
}