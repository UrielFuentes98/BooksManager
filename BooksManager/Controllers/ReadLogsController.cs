using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BooksManager.Data;
using BooksManager.Models;

namespace BooksManager.Controllers
{
    public class ReadLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReadLogsRepository readLogsRepository;

        public ReadLogsController(ApplicationDbContext context, IReadLogsRepository readLogsRepository)
        {
            _context = context;
            this.readLogsRepository = readLogsRepository;
        }

        // GET: ReadLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ReadLogs.Include(r => r.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ReadLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readLog = await _context.ReadLogs
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.ReadLogId == id);
            if (readLog == null)
            {
                return NotFound();
            }

            return View(readLog);
        }

        // GET: ReadLogs/Create
        public IActionResult Create(int bookId, string bookName)
        {
            ViewData["BookId"] = bookId;
            ViewData["BookName"] = bookName;
            return View();
        }

        // POST: ReadLogs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ReadLogId,PageNumber,LogDate,Note")] ReadLog readLog, int bookId)
        {
            if (ModelState.IsValid)
            {
                readLog.BookId = bookId;
                readLogsRepository.AddLog(readLog);
                return RedirectToAction("Detail","Book", new { bookId });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            return View(readLog);
        }

        // GET: ReadLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readLog = await _context.ReadLogs.FindAsync(id);
            if (readLog == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            return View(readLog);
        }

        // POST: ReadLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReadLogId,PageNumber,LogDate,Note,BookId")] ReadLog readLog)
        {
            if (id != readLog.ReadLogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadLogExists(readLog.ReadLogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            return View(readLog);
        }

        // GET: ReadLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readLog = await _context.ReadLogs
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.ReadLogId == id);
            if (readLog == null)
            {
                return NotFound();
            }

            return View(readLog);
        }

        // POST: ReadLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var readLog = await _context.ReadLogs.FindAsync(id);
            _context.ReadLogs.Remove(readLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReadLogExists(int id)
        {
            return _context.ReadLogs.Any(e => e.ReadLogId == id);
        }
    }
}
