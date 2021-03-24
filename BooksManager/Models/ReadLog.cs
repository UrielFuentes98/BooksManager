using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class ReadLog
    {
        public int ReadLogId { get; set; }

        [Required(ErrorMessage = "Please enter the page number for the log.")]
        [Display(Name = "Page Number")]
        public int PageNumber { get; set; }

        [Required]
        [Display(Name = "Date of Log")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Column(TypeName = "Date")]
        public DateTime LogDate { get; set; }

        public string Note { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
