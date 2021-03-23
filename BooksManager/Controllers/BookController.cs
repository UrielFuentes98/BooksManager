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
            if (bookId > 0)
            {
                var bookToEdit = booksRepository.GetBookById(bookId);
                return View(bookToEdit);
            }
            else
            {
                var newBook = new Book();
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

            if (bookReturned.BookId > 0)
            {
                booksRepository.UpdateBook(bookReturned);
            }
            else
            {
                bookReturned.UserName = User.Identity.Name;
                booksRepository.AddBook(bookReturned);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
