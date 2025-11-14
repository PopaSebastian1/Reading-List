using Reading_List.Domain.Enums;
using Reading_List.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Domain.Models
{
    public class Book : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public Genre Genre { get; set; }
        public bool Finished { get; set; }
        public decimal? Rating { get; set; }


        public Book(int id, string title, string author, int year, int pages, Genre genre, bool finished, decimal? rating)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
            Pages = pages;
            Genre = genre;
            Finished = finished;
            Rating = rating;
        }

        public override string ToString()
        {
            return $"{Id}: {Title} by {Author} ({Year}) - {Pages} pages - Genre: {Genre} - Finished: {Finished}" +
                   (Finished && Rating.HasValue ? $" - Rating: {Rating}/5" : "");
        }


    }
}
