using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class ReadLog
    {
        public int ReadLogId { get; set; }

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public DateTime LogDate { get; set; }

        public string Note { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
