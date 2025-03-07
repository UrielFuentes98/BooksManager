﻿using BooksManager.Models;
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
        public IActionResult Edit(int id, BookStatus newStatus)
        {
            //If bookId passed as parameter search book, else create one
            if (id > 0)
            {
                var bookToEdit = booksRepository.GetBookById(id);
                return View(bookToEdit);
            }
            else
            {
                var newBook = new Book() { Status = newStatus, ReadLogs = new List<ReadLog>() };
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
                            new { id = bookReturned.BookId, bookName = bookReturned.Name });
                        
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

        public IActionResult Detail (int id)
        {
            var bookDetail = booksRepository.GetBookById(id);
            var detailsWithStats = booksRepository.AddStats(bookDetail);
            ViewData["itemsClass"] = "detail-entry";
            return View(detailsWithStats);
        }

        public IActionResult DeletePreview(int id)
        {
            var bookToDelete = booksRepository.GetBookById(id);
            var bookWithStats = booksRepository.AddStats(bookToDelete);
            return View("Delete/DeletePreview", bookWithStats);
        }

        public IActionResult DeleteConfirmation(int id)
        {
            var bookToDelete = booksRepository.GetBookById(id);
            booksRepository.DeleteBook(bookToDelete);
            return View("Delete/DeleteConfirmation");
        }

        public IActionResult Logs (int id)
        {
            var bookOfLogs = booksRepository.GetBookById(id);

            //Order by date for correct display on table
            bookOfLogs.ReadLogs = bookOfLogs.ReadLogs.OrderBy(l => l.PageNumber).ToList();



            return View(bookOfLogs);
        }

        public IActionResult StartToRead(int id)
        {
            //Update status to currently reading and redirect to create first log
            var bookToUpdate = booksRepository.GetBookById(id);
            booksRepository.UpdateBook(bookToUpdate);

            return RedirectToAction("Create", "ReadLogs",
                            new { id = bookToUpdate.BookId, bookName = bookToUpdate.Name });

        }

        public IActionResult FinishReading(int id)
        {
            var bookFinished = booksRepository.GetBookById(id);
            
            //Create finish log for book
            var finishLog = new ReadLog()
            { Book = bookFinished, LogDate = DateTime.Today, PageNumber = bookFinished.NumberOfPages, Note = "Finish Log." };

            //Add log, save book and update status
            bookFinished.ReadLogs.Add(finishLog);
            bookFinished.Status = BookStatus.Read;
            booksRepository.UpdateBook(bookFinished);

            return RedirectToAction("Index", "Home");
        }

    }
}
