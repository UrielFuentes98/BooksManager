using BooksManager.Data;
using Microsoft.EntityFrameworkCore;
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

        public void AddBook(Book book)
        {
            dbContext.Books.Add(book);
            dbContext.SaveChanges();
        }

        public void DeleteBook(Book book)
        {
            dbContext.Books.Remove(book);
            dbContext.SaveChanges();
        }

        public Book GetBookById(int bookId)
        {
            return dbContext.Books.Include(b => b.ReadLogs).Single(b => b.BookId == bookId);
        }

        public IEnumerable<Book> GetBooksFromUser(string userName)
        {
            return dbContext.Books.Where(b => b.UserName == userName);
        }

        public void UpdateBook(Book book)
        {
            dbContext.Books.Update(book);
            dbContext.SaveChanges();
        }
    }
}
