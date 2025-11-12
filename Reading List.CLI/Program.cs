using Microsoft.Extensions.DependencyInjection;
using Reading_List.Application.Abstractions;
using Reading_List.Domain.Models;
using Reading_List.Infrastructure.Repositories;
using Reading_List.Infrastructure.FileService;
using System;
using System.IO;
using System.Threading.Tasks;


/// <summary>
/// This is a test Main for import and export functionality.
/// </summary>
class Program
{
    static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IRepository<Book>>(new InMemoryRepository<int, Book>(b => b.Id));
        services.AddSingleton<IBookImportExportService, BookImportExportService>();
        var provider = services.BuildServiceProvider();

        var importExport = provider.GetRequiredService<IBookImportExportService>();

        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\"));
        var basePath = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");

        var file1 = Path.Combine(basePath, "books_part1.csv");
        var file2 = Path.Combine(basePath, "books_part2.csv");

        if (!File.Exists(file1) || !File.Exists(file2))
        {
            Console.WriteLine(" Missing CSV files in 'Reading List.Infrastructure/Data' folder.");
            Console.WriteLine($"Expected:\n  {file1}\n  {file2}");
            return;
        }

        if (!File.Exists(file1) || !File.Exists(file2))
        {
            Console.WriteLine(" Missing CSV files in 'Infrastructure/Data' folder.");
            Console.WriteLine($"Expected: \n  {file1}\n  {file2}");
            return;
        }

        var files = new[] { file1, file2 };

        var importResult = await importExport.ImportAsync(files);
        Console.WriteLine($"Imported {importResult.Value} books.");

        var exportPath = Path.Combine(basePath, "exported_books.json");
        var exportResult = await importExport.ExportAsync(exportPath);
        Console.WriteLine(exportResult.IsSuccess
            ? $" Export successful! File saved at:\n  {exportPath}"
            : " Export failed.");
    }
}
