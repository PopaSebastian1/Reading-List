using Reading_List.Domain.Enums;
using Reading_List.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Dtos
{
    public class BookDto : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public Genre Genre { get; set; }

        public int PageCount { get; set; }

        public bool  Finished { get;set; } 
        public decimal? Rating { get; set; }

        public override string ToString()
        {
            string finishedText = Finished ? "Yes" : "No";
            return $"{Id}: {Title} by {Author} - {PageCount} pages - Genre: {Genre} - Finished: {finishedText}" +
                   (Finished && Rating.HasValue ? $" - Rating: {Rating}/5" : "");
        }

    }
}
