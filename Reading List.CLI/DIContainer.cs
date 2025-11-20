using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Reading_List.Application.Abstractions;
using Reading_List.Application.Commands;
using Reading_List.Application.Mappers;
using Reading_List.Application.Services;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;
using Reading_List.Domain.Validators;
using Reading_List.Infrastructure.FileService;
using Reading_List.Infrastructure.Repositories;

namespace Reading_List.CLI
{
    public static class DIContainer
    {
        public static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IRepository<Book>>(new InMemoryRepository<int, Book>(b => b.Id));
            services.AddSingleton<IBookImportHandler, BookImportHandler>();
            services.AddSingleton<IBookExportStrategy, JsonBookExportStrategy>();
            services.AddSingleton<IBookExportStrategy, CsvBookExportStrategy>();
            services.AddSingleton<IBookExportHandler, BookExportHandler>();
            services.AddSingleton<ILoggerService, LoggerService>();

            services.AddSingleton<IMapper<Book, BookDto>, BookMapper>();
            services.AddSingleton<IValidator<BookDto>, BookValidator>();
            services.AddSingleton<IBookService, BookService>();

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

            return services.BuildServiceProvider();
        }
    }
}