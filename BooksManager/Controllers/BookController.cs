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
        public IActionResult Edit(int bookId, BookStatus newStatus)
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
                var newBook = new Book() { Status = newStatus };
                return View(newBook);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Book bookReturned)
        {
            //If model is not valid return to inform the user
            if (!ModelState.IsValid)
            {
                return View();
            }

            //If book has valid id, update it, else create it.
            if (bookReturned.BookId > 0)
            {
                booksRepository.UpdateBook(bookReturned);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                bookReturned.UserName = User.Identity.Name;

                switch(bookReturned.Status)
                {
                    case BookStatus.ToRead:
                        booksRepository.AddBook(bookReturned);
                        return RedirectToAction("Index", "Home");

                    case BookStatus.CurrentlyReading:
                        booksRepository.AddBook(bookReturned);
                        //Ask for initial log of book
                        return RedirectToAction("Create", "ReadLogs",
                            new { bookId = bookReturned.BookId, bookName = bookReturned.Name });
                        
                    case BookStatus.Read:
                        //Create finish log for book
                        var finishLog = new ReadLog()
                        { Book = bookReturned, LogDate = DateTime.Today, PageNumber = bookReturned.NumberOfPages };
                        
                        //Add log and save book
                        bookReturned.ReadLogs.Add(finishLog);
                        booksRepository.AddBook(bookReturned);
                        return RedirectToAction("Index", "Home");
                    default:
                        return RedirectToAction("Index", "Home");
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
            bookOfLogs.ReadLogs = bookOfLogs.ReadLogs.OrderBy(l => l.PageNumber).ToList();



            return View(bookOfLogs);
        }

    }
}
