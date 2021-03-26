using BooksManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.ViewsModels
{
    public class BookWithStats
    {
        public Book Book { get; set; }

        public int DaysReading { get; set; }
        public int ProgressPercentage { get; set; }

        public int LastPageRead { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateFinished { get; set; }
    }
}
