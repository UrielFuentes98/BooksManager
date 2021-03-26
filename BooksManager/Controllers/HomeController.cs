using BooksManager.Models;
using BooksManager.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBooksRepository booksRepository;

        public HomeController(ILogger<HomeController> logger, IBooksRepository booksRepository)
        {
            _logger = logger;
            this.booksRepository = booksRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            var bookList = booksRepository.GetBooksFromUser(User.Identity.Name);
            var bookWithStatsList = new List<BookWithStats>();

            //Calculate stats for each book and store in list
            foreach (var book in bookList)
            {
                bookWithStatsList.Add(calculateStats(book));
            }

            var booksSorted = from book in bookWithStatsList
                             orderby book.DateFinished
                             select book;

            return View(booksSorted);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        BookWithStats calculateStats (Book book)
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

                _logger.LogInformation(book.Name);
                _logger.LogInformation(book.Status.ToString());
                _logger.LogInformation(firstDate.ToString());
            }

            return bookWithStats;
        }
    }
}
