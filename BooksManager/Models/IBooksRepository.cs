using BooksManager.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public interface IBooksRepository
    {
        IEnumerable<Book> GetBooksFromUser(string userName);

        Book GetBookById(int bookId);

        void AddBook(Book book);

        void UpdateBook(Book book);

        void DeleteBook(Book book);

        void UpdateBookStatus(int bookId);

        BookWithStats AddStats(Book book);
    }
}
