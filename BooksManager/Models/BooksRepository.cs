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

        public void UpdateBookStatus(int bookId)
        {
            var bookToUpdate = dbContext.Books.Include(b => b.ReadLogs).SingleOrDefault(b => b.BookId == bookId);
            var bookHasLogs = bookToUpdate.ReadLogs.Any();

            if (bookHasLogs)
            {
                var hasFinishLog = bookToUpdate.ReadLogs.Any(l => l.PageNumber == bookToUpdate.NumberOfPages);

                //Check if status should be Read and updte it if not 
                if (hasFinishLog && bookToUpdate.Status != BookStatus.Read)
                {
                    bookToUpdate.Status = BookStatus.Read;
                    UpdateBook(bookToUpdate);
                }
                //Status should be CurrentlyReading, updte it if not
                else if (bookToUpdate.Status != BookStatus.CurrentlyReading)
                {
                    bookToUpdate.Status = BookStatus.CurrentlyReading;
                    UpdateBook(bookToUpdate);
                }
            }
            //Status should be ToRead, update it if not
            else if (bookToUpdate.Status != BookStatus.ToRead)
            {
                bookToUpdate.Status = BookStatus.ToRead;
                UpdateBook(bookToUpdate);
            }
        }
    }
}
