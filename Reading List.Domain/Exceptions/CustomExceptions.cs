using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Exceptions
{
    public class CustomExceptions : Exception
    {
        public CustomExceptions(string message) : base(message)
        {
        }

    }
    
    public class EntityNotFoundException : CustomExceptions
    {
        public int EntityId { get; }
        public EntityNotFoundException(int id) : base($"Entity with ID {id} not found.")
        {
        }

    }

    public class DuplicateEntityException : CustomExceptions
    {
        public int EntityId { get; }
        public DuplicateEntityException(int id) : base($"Entity with ID {id} already exists.")
        {
        }
    }

    public class InvalidEntityException : CustomExceptions
    {
        public InvalidEntityException(string message) : base(message)
        {
        }
    }

    public class RatingOutOfRangeException : CustomExceptions
    {
        public decimal Rating { get; }
        public RatingOutOfRangeException(decimal rating) : base($"Rating  must be between 1 and 5.")
        {
            Rating = rating;
        }
    }

    public sealed class InvalidInputException : CustomExceptions
    {
        public InvalidInputException(string message) : base(message) { }
    }

    public sealed class UnsupportedExportFormatException : CustomExceptions
    {
        public UnsupportedExportFormatException(string ext) : base($"Export format '{ext}' is not supported.") { }
    }

    public sealed class ImportFileMissingException : CustomExceptions
    {
        public ImportFileMissingException(string path) : base($"Import file not found: {path}") { }
    }

    public sealed class MalformedCsvRowException : CustomExceptions
    {
        public MalformedCsvRowException(string raw) : base($"Malformed CSV row: {raw}") { }
    }

}
