using BooksManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class BooksRepository : IBooksRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BooksRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Book> GetBooksFromUser(string userName)
        {
            return dbContext.Books.Where(b => b.UserName == userName);
        }
    }
}
