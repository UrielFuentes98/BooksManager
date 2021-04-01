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
                bookWithStatsList.Add(booksRepository.AddStats(book));
            }

            var booksSorted = from book in bookWithStatsList
                             orderby book.DateFinished
                             select book;
            ViewData["itemsClass"] = "card-entry";
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


    }
}
