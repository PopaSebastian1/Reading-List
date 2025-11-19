using FluentValidation;
using Reading_List.Domain.Dtos;
using Reading_List.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Validators
{
    public class BookValidator : AbstractValidator<BookDto>
    {
        public BookValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Author).NotEmpty().WithMessage("Author is required.");
            RuleFor(x => x.Rating).InclusiveBetween(1m,5.0m).When(x=> x.Rating.HasValue).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.PageCount).GreaterThan(0).WithMessage("Pages must be greater than 0.");

        }
    }
}
