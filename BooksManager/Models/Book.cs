using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Author { get; set; }

        [Required]
        public int NumberOfPages { get; set; }

        public string Note { get; set; }

        public List<ReadLog> ReadLogs { get; set; }
    }
}
