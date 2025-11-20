using Reading_List.Domain.Models;
using Reading_List.Infrastructure.FileService;
using Reading_List.Infrastructure.Repositories;
using Reading_List.Tests.TestsHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Reading_List.Tests.Tests
{
    public class BookImportTests
    {
        private readonly InMemoryRepository<int, Book> repo;
        private readonly TestLogger log;
        private readonly BookImportService bookImportService;

        public BookImportTests()
        {
            repo = new InMemoryRepository<int, Book>(b => b.Id);
            log = new TestLogger();
            bookImportService = new BookImportService(repo, log);
        }

        [Fact]
        public async Task Import_SingleValidFile_ImportsAll_and_LogsSummary()
        {
            var csv = """
            Id,Title,Author,Year,Pages,Genre,Finished,Rating
            1,A,B,2020,100,Software,true,4.5
            2,C,D,2021,200,Fantasy,false,3.7
            """;

            var path = TestFileHelper.CreateTempCsv("valid", csv);
            var result = await bookImportService.ImportAsync(new[] { path });

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value);

            var all = (await repo.GetAllAsync())
                .Where(r => r.IsSuccess)
                .Select(r => r.Value!)
                .ToList();

            Assert.Equal(2, all.Count);
            Assert.Empty(log.Errors);
            Assert.Empty(log.Warnings);
            Assert.Contains(log.Infos, m => m.Contains("Import summary"));
        }

        [Fact]
        public async Task Import_MissingFile_LogsError_and_ContinuesWithExisting()
        {
            var existing = TestFileHelper.CreateTempCsv("valid", """
            Id,Title,Author,Year,Pages,Genre,Finished,Rating
            1,A,B,2020,100,Software,true,4.5
            """);

            var missing = existing + "_missing.csv";
            var res = await bookImportService.ImportAsync(new[] { existing, missing });

            Assert.True(res.IsSuccess);
            Assert.Equal(1, res.Value);
            Assert.Single(log.Errors);
            Assert.Contains("File not found", log.Errors[0]);
        }

        [Fact]
        public async Task Import_MalformedRow_LogsError_and_SkipsRow()
        {
            var path = TestFileHelper.CreateTempCsv("badrow", """
            Id,Title,Author,Year,Pages,Genre,Finished,Rating
            1,Good,A,2020,100,Software,true,4.5
            X,Bad,B,YYYY,foo,Software,maybe,NaN
            2,Good2,C,2021,120,Fantasy,false,4.0
            """);

            var res = await bookImportService.ImportAsync(new[] { path });

            Assert.True(res.IsSuccess);
            Assert.Equal(2, res.Value);
            Assert.NotEmpty(log.Errors);
            Assert.Contains(log.Errors, e => e.Contains("Invalid row"));
        }

        [Fact]
        public async Task Import_InvalidEnum_LogsError_and_SkipsRow()
        {
            var path = TestFileHelper.CreateTempCsv("badenum", """
            Id,Title,Author,Year,Pages,Genre,Finished,Rating
            1,Good,A,2022,111,Software,true,4.5
            3,WrongGenre,Z,2023,222,NotAGenre,true,4.2
            """);

            var res = await bookImportService.ImportAsync(new[] { path });

            Assert.True(res.IsSuccess);
            Assert.Equal(1, res.Value);
            Assert.NotEmpty(log.Errors);
            Assert.Contains(log.Errors, e => e.Contains("Invalid row"));
        }

        [Fact]
        public async Task Import_HeaderCaseInsensitive_Works()
        {
            var path = TestFileHelper.CreateTempCsv("headers", """
            id,title,author,year,pages,genre,finished,rating
            1,a,b,2020,100,Software,true,4.5
            """);

            var res = await bookImportService.ImportAsync(new[] { path });

            Assert.True(res.IsSuccess);
            Assert.Equal(1, res.Value);
            Assert.Empty(log.Errors);
        }

        [Fact]
        public async Task Import_EmptyFile_ImportsZero_and_LogsSummary()
        {
            var path = TestFileHelper.CreateTempCsv("empty", "Id,Title,Author,Year,Pages,Genre,Finished,Rating\n");

            var res = await bookImportService.ImportAsync(new[] { path });

            Assert.True(res.IsSuccess);
            Assert.Equal(0, res.Value);
            Assert.Contains(log.Infos, m => m.Contains("Import summary"));
        }
    }
}
