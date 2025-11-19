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
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Pages { get; set; }
        public Genre Genre { get; set; }
        public string Finished { get; set; } = string.Empty;
        public decimal? Rating { get; set; }

        public bool isFinished()
           {
            var trueVariants = new HashSet<string>
            {
                "yes", "true", "1", "y", "t"
            };
            return trueVariants.Contains(Finished.Trim().ToLower());
        }




    }
}
