using FluentValidation;
using FluentValidation.Results;
using Reading_List.Application.Services;
using Reading_List.Domain.Enums;
using Reading_List.Domain.Models;
using Reading_List.Domain.Validators;
using Reading_List.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Reading_List.Tests.Tests
{
    [Collection("BookServiceTests")]
    public sealed class BookServiceTests
    {
        private static BookService NewSvc(IValidator<Book>? validator = null)
        {
            var repo = new InMemoryRepository<int, Book>(b => b.Id);
            validator ??= new BookValidator();
            return new BookService(repo, validator);
        }

        private static Book B(int id, string? title = null, string? author = null, Genre genre = Genre.Fiction, bool finished = false, decimal? rating = null, int pages = 100, int year = 2020)
            => new(id, title ?? $"Title {id}", author ?? "Author", year, pages, genre, finished, rating);

        [Fact]
        public async Task AddAsync_MissingTitle_FailsWithMessage()
        {
            var svc = NewSvc();
            var book = new Book(1, "", "Author", 2024, 100, Genre.Fiction, false, null);

            var result = await svc.AddAsync(book);

            Assert.False(result.IsSuccess);
            Assert.Contains("Title is required.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_MissingAuthor_FailsWithMessage()
        {
            var svc = NewSvc();
            var book = new Book(2, "Title", "", 2024, 100, Genre.Fiction, false, null);

            var result = await svc.AddAsync(book);

            Assert.False(result.IsSuccess);
            Assert.Contains("Author is required.", result.ErrorMessage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5.5)]
        public async Task UpdateAsync_RatingOutOfRange_Fails(decimal invalidRating)
        {
            var svc = NewSvc();
            var book = new Book(3, "T", "A", 2024, 123, Genre.Fiction, false, invalidRating);

            var added = await svc.AddAsync(new Book(4, "T", "A", 2024, 100, Genre.Fiction, false, null));
            var stored = (await svc.GetByIdAsync(4)).Value!;
            stored.Rating = invalidRating;

            var result = await svc.UpdateAsync(stored);

            Assert.False(result.IsSuccess);
            Assert.Contains("Rating must be between 0 and 5.", result.ErrorMessage);
        }


        [Fact]
        public async Task AddAsync_Valid_Adds_And_GetAllReturnsIt()
        {
            var svc = NewSvc();
            var add = await svc.AddAsync(B(1));
            Assert.True(add.IsSuccess);

            var all = await svc.GetAllAsync();
            var items = all.Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!).ToList();
            Assert.Single(items);
            Assert.Equal(1, items[0].Id);
        }

        [Fact]
        public async Task AddAsync_Duplicate_ReturnsFailure()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1));
            var dup = await svc.AddAsync(B(1));
            Assert.False(dup.IsSuccess);
            Assert.Contains("already", dup.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1, title: "Old"));
            var book = (await svc.GetByIdAsync(1)).Value!;
            book.Title = "New";
            var up = await svc.UpdateAsync(book);
            Assert.True(up.IsSuccess);
            Assert.Equal("New", (await svc.GetByIdAsync(1)).Value!.Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            var svc = NewSvc();
            var b = B(1);
            await svc.AddAsync(b);
            var del = await svc.DeleteAsync(b);
            Assert.True(del.IsSuccess);

            var get = await svc.GetByIdAsync(1);
            Assert.False(get.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsFailure()
        {
            var svc = NewSvc();
            var r = await svc.GetByIdAsync(999);
            Assert.False(r.IsSuccess);
            Assert.Contains("was not found", r.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetFinishedBooks_ReturnsOnlyFinished()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1, finished: true));
            await svc.AddAsync(B(2, finished: false));
            await svc.AddAsync(B(3, finished: true));

            var finished = await svc.GetFinishedBooks();
            var list = finished.Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!).ToList();

            Assert.Equal(2, list.Count);
            Assert.All(list, b => Assert.True(b.Finished));
        }

        [Fact]
        public async Task GetTopRatedBooks_ReturnsDescending_And_RespectsDefaultCount()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1, rating: 4.1m));
            await svc.AddAsync(B(2, rating: 3.7m));
            await svc.AddAsync(B(3, rating: 4.9m));
            await svc.AddAsync(B(4, rating: 4.5m));
            await svc.AddAsync(B(5, rating: null)); 

            var top = await svc.GetTopRatedBooks(5);
            var topList = top.Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!).ToList();

            Assert.Equal(new[] { 4.9m, 4.5m, 4.1m, 3.7m }, topList.Select(b => b.Rating!.Value).ToArray());
        }

        [Fact]
        public async Task GetBooksByAuthor_IsCaseInsensitive()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1, author: "Alice"));
            await svc.AddAsync(B(2, author: "alice"));
            await svc.AddAsync(B(3, author: "Bob"));

            var res = await svc.GetBooksByAuthor("ALICE");
            var list = res.Where(r => r.IsSuccess && r.Value is not null).Select(r => r.Value!).ToList();

            Assert.Equal(2, list.Count);
            Assert.All(list, b => Assert.Equal("Alice", b.Author, ignoreCase: true));
        }

        [Fact]
        public async Task MarkAsFinished_SetsFinishedTrue()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(10, finished: false));
            var r = await svc.MarkAsFinished(10);
            Assert.True(r.IsSuccess);
            Assert.True(r.Value!.Finished);
        }

        [Fact]
        public async Task SetRating_OutOfBounds_ReturnsFailure()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1));
            var r1 = await svc.SetRating(1, 0.9m);
            var r2 = await svc.SetRating(1, 5.1m);
            Assert.False(r1.IsSuccess);
            Assert.False(r2.IsSuccess);
            Assert.Contains("between 1 and 5", r1.ErrorMessage!);
        }

        [Fact]
        public async Task SetRating_Valid_SetsFinishedAndRating()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(2, finished: false));
            var r = await svc.SetRating(2, 4.5m);
            Assert.True(r.IsSuccess);
            Assert.True(r.Value!.Finished);
            Assert.Equal(4.5m, r.Value!.Rating);
        }

        [Fact]
        public async Task GetStatsAsync_ComputesTotals_Finished_AvgAndPagesByGenre()
        {
            var svc = NewSvc();
            await svc.AddAsync(B(1, genre: Genre.Fantasy, finished: true, rating: 4.0m, pages: 300));
            await svc.AddAsync(B(2, genre: Genre.Fantasy, finished: false, rating: 5.0m, pages: 200));
            await svc.AddAsync(B(3, genre: Genre.Software, finished: true, rating: null, pages: 150));

            var stats = await svc.GetStatsAsync();
            Assert.True(stats.IsSuccess);

            var s = stats.Value!;
            Assert.Equal(3, s.TotalBooks);
            Assert.Equal(2, s.FinishedCount);
            Assert.Equal(4.5m, s.AverageRating);
            Assert.Equal(500, s.PagesByGenre[Genre.Fantasy]);
            Assert.Equal(150, s.PagesByGenre[Genre.Software]);
        }
    }
}
