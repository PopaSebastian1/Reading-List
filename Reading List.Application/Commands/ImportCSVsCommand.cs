using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Models;

namespace Reading_List.Application.Commands
{
    public sealed class ImportCSVsCommand : ICommand
    {
        public string Key => "1";
        public string Description => "Import one or more CSV files (comma-separated).";

        private readonly IBookImportHandler _importHandler;

        public ImportCSVsCommand(IBookImportHandler bookImportExportService)
        {
            _importHandler = bookImportExportService;
        }

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");

            var input = ConsoleInputHandler.ReadNonEmptyString("Enter CSV file paths separated by commas: ", ct) ?? string.Empty;   

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
                importResult = await _importHandler.ImportAsync(resolved, ct);
            }
            catch (Exception ex)
            {
                importResult = ErrorHandler.GenericError<int>($"An error occurred during import: {ex.Message}");
            }

            return importResult.IsSuccess
                ? $"Successfully imported {importResult.Value} book(s)."
                : $"Import failed: {importResult.ErrorMessage}";
        }
    }
}