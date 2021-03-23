using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public interface IBooksRepository
    {
        IEnumerable<Book> GetBooksFromUser(string userName);
    }
}
