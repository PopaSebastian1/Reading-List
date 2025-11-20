using Reading_List.Application.Abstractions;
using Reading_List.Application.Handlers;
using Reading_List.Domain.Exceptions;

namespace Reading_List.Application.Commands
{
    public class SetRatingCommand : ICommand
    {
        public string Key => "7";
        public string Description => "Rate a Book";

        private readonly IBookService _bookService;

        public SetRatingCommand(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<string> ExecuteAsync(CancellationToken ct = default)
        {
            var id = ConsoleInputHandler.ReadInt("Enter the Book ID to rate: ", i => i > 0, ct);

            var bookResult = await _bookService.GetByIdAsync(id);
            if (!bookResult.IsSuccess || bookResult.Value is null)
                throw new EntityNotFoundException(id);

            var rating = ConsoleInputHandler.ReadDecimal("Enter your rating (1-5): ", v => v is >= 1m and <= 5m, ct);
            try
            {
                var result = await _bookService.SetRating(id, rating);

                return result.IsSuccess
                    ? $"Book ID {id} rated {rating} successfully. -> 200 Ok"
                    : $"Failed to rate Book ID {id}: {result.ErrorMessage}";
            }
            catch (EntityNotFoundException ex)
            {
                return $"Book ID {id} not found -> 404 Not Found";
            }
            catch (RatingOutOfRangeException ex)
            {
                return $"Invalid rating {ex.Rating}: {ex.Message} -> 400 Bad Request";
            }
            catch (Exception ex)
            {
                return $"An error occurred while rating Book ID {id}: {ex.Message} -> 500 Internal Server Error";
            }
        }
    }

}