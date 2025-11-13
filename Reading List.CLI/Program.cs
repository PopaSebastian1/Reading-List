using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Commands;
using Reading_List.Application.Services;
using Reading_List.CLI;
using Reading_List.Domain.Models;
using Reading_List.Domain.Validators;
using Reading_List.Infrastructure.FileService;
using Reading_List.Infrastructure.Repositories;
using System;

class Program
{
    static async Task Main()
    {
        var services = new ServiceCollection();


        services.AddSingleton<IRepository<Book>>(new InMemoryRepository<int, Book>(b => b.Id));
        services.AddSingleton<IBookImportService, BookImportService>();
        services.AddSingleton<IBookExportStrategy, JsonBookExportStrategy>();
        services.AddSingleton<IBookExportStrategy, CsvBookExportStrategy>();
        services.AddSingleton<IBookExportService, BookExportService>();
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<IBookService,BookService>();

        services.AddSingleton<IValidator<Book>, BookValidator>();


        services.AddSingleton<ICommand, ImportCSVsCommand>();
        services.AddSingleton<ICommand, ListAllCommand>();
        services.AddSingleton<ICommand, GetTopRatedCommand>();
        services.AddSingleton<ICommand, GetFinishedCommand>();
        services.AddSingleton<ICommand, GetBooksByAuthor>();
        services.AddSingleton<ICommand, MarkAsFinishedCommand>();
        services.AddSingleton<ICommand, SetRatingCommand>();
        services.AddSingleton<ICommand, ClearConsoleCommand>();
        services.AddSingleton<ICommand, ExportBooksCommand>();
        services.AddSingleton<ICommand, GetStatsCommand>();


        services.AddSingleton<Menu>();

        var provider = services.BuildServiceProvider();
        var menu = provider.GetRequiredService<Menu>();
        await menu.RunAsync();
    }
}