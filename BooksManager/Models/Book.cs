using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class Book
    {

        public Book()
        {

        }

        public int BookId { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a Name for the book.")]
        public string Name { get; set; }

        public string Author { get; set; }

        [Required(ErrorMessage = "Please enter the number of pages of the book.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Pages must be positve.")]
        [Display(Name = "Number of Pages")]
        public int NumberOfPages { get; set; }

        public string Note { get; set; }

        public BookStatus Status { get; set; }

        public List<ReadLog> ReadLogs { get; set; } = new List<ReadLog>();
        public Book Include { get; internal set; }
    }
}
