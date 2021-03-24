using BooksManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Controllers
{
    public class BookController : Controller
    {
        private readonly IBooksRepository booksRepository;

        public BookController(IBooksRepository booksRepository)
        {
            this.booksRepository = booksRepository;
        }

        [Authorize]
        public IActionResult Edit(int bookId)
        {
            //If bookId passed as parameter search book, else create one
            if (bookId > 0)
            {
                ViewData["Mode"] = "Edit";
                var bookToEdit = booksRepository.GetBookById(bookId);
                return View(bookToEdit);
            }
            else
            {
                ViewData["Mode"] = "Create";
                var newBook = new Book();
                return View(newBook);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Book bookReturned, string reading)
        {
            //If model is not valid return to inform the user
            if (!ModelState.IsValid)
            {
                return View();
            }

            //If book has valid id, update it, else delete it.
            if (bookReturned.BookId > 0)
            {
                booksRepository.UpdateBook(bookReturned);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                bookReturned.UserName = User.Identity.Name;

                //If not currently reading, add book and return home,
                //else add book and redirect to new log
                if (String.IsNullOrEmpty(reading))
                {
                    bookReturned.Status = BookStatus.CurrentlyReading;
                    booksRepository.AddBook(bookReturned);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    bookReturned.Status = BookStatus.ToRead;
                    booksRepository.AddBook(bookReturned);
                    return RedirectToAction("Create", "ReadLogs", 
                        new { bookId = bookReturned.BookId, bookName = bookReturned.Name });
                }
            }
            
        }

        public IActionResult Detail (int bookId)
        {
            var bookDetail = booksRepository.GetBookById(bookId);
            return View(bookDetail);
        }

        public IActionResult DeletePreview(int bookId)
        {
            var bookToDelete = booksRepository.GetBookById(bookId);
            return View("Delete/DeletePreview", bookToDelete);
        }

        public IActionResult DeleteConfirmation(int bookId)
        {
            var bookToDelete = booksRepository.GetBookById(bookId);
            booksRepository.DeleteBook(bookToDelete);
            return View("Delete/DeleteConfirmation");
        }

        public IActionResult Logs (int bookId, string bookName)
        {
            var bookOfLogs = booksRepository.GetBookById(bookId);

            //Order by date for correct display on table
            bookOfLogs.ReadLogs = bookOfLogs.ReadLogs.OrderBy(l => l.LogDate).ToList();



            return View(bookOfLogs);
        }

    }
}
