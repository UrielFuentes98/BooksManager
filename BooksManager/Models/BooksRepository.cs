using BooksManager.Data;
using BooksManager.ViewsModels;
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
            return dbContext.Books.Include(b => b.ReadLogs).Where(b => b.UserName == userName);
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

        public BookWithStats AddStats(Book book)
        {
            var bookWithStats = new BookWithStats
            {
                Book = book
            };

            if (book.Status != BookStatus.ToRead)
            {
                //Get logs of book sorted.
                var logsSorted = from log in book.ReadLogs
                                 orderby log.PageNumber
                                 select log;

                //Get date of first log entered and last page entered
                var firstDate = logsSorted.First().LogDate;
                var lastPage = logsSorted.Last().PageNumber;

                bookWithStats.LastPageRead = lastPage;
                bookWithStats.DateFinished = logsSorted.Last().LogDate;

                //Calcualte days reading and percentage of progress
                var dateDifference = DateTime.Now.Date - firstDate.Date;
                bookWithStats.DaysReading = dateDifference.Days;

                bookWithStats.ProgressPercentage = (lastPage * 100) / book.NumberOfPages;

                //Calculating pages per week.
                if (bookWithStats.DaysReading >= 7)
                {
                    bookWithStats.PagesPerWeek = (int) (bookWithStats.LastPageRead / ((double)bookWithStats.DaysReading / 7));
                }
                else
                {
                    bookWithStats.PagesPerWeek = bookWithStats.LastPageRead;
                }

            }

            return bookWithStats;
        }

    }
}
