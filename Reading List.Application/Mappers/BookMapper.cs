using Reading_List.Application.Abstractions;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Mappers
{
    public sealed class BookMapper: IMapper<Book, BookDto>
    {
        public BookDto toDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PageCount = book.Pages,
                Finished = ConvertStringToBool(book.Finished),
                Rating = book.Rating
            };
        }
        public Book toEntity(BookDto bookDto)
        {
            return new Book
            {
                Id = bookDto.Id,
                Title = bookDto.Title,
                Author = bookDto.Author,
                Genre = bookDto.Genre,
                Pages = bookDto.PageCount,
                Finished = bookDto.Finished ? "yes" : "no",
                Rating = bookDto.Rating
            };
        }
        private bool ConvertStringToBool(string value)
        {
            var trueVariants = new HashSet<string>
            {
                "yes", "true", "1", "y", "t"
            };
            return trueVariants.Contains(value.Trim().ToLower());

        }
    }
}
