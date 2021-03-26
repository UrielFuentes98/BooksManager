using BooksManager.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public class ReadLogsRepository : IReadLogsRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ReadLogsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private bool validateLog(ReadLog log)
        {
            var newBook = dbContext.Books.AsNoTracking().Include(b => b.ReadLogs).SingleOrDefault(b => b.BookId == log.BookId);
            var previousLogs = newBook.ReadLogs.OrderBy(l => l.PageNumber).ToList();

            //If no other logs only check page number is between number of pages of book
            if (previousLogs.Count() == 0)
            {
                if (log.PageNumber <= newBook.NumberOfPages)
                {
                    return true;
                }
            }
            else
            {
                for (var index = 0; index < previousLogs.Count(); index++)
                {
                    if (log.PageNumber < previousLogs[index].PageNumber)
                    {
                        if (index > 0)
                        {
                            //Only check lower bound if it exist.
                            //Log with lower page num has to have earlier or equal date.
                            if (previousLogs[index - 1].LogDate.Date > log.LogDate.Date)
                            {
                                return false;
                            }
                        }
                        //Check upper bound.
                        //Log with higher page num has to have later or equal date.
                        if (previousLogs[index].LogDate.Date < log.LogDate.Date)
                        {
                            return false;
                        }

                        //Both checks passed
                        return true;
                    }

                    //New log has highest page num, check lower bound and number of pages
                    if (index == previousLogs.Count() - 1)
                    {
                        //Log with lower page num has to have earlier or equal date.
                        if (previousLogs[index].LogDate.Date > log.LogDate.Date)
                        {
                            return false;
                        }

                        if (log.PageNumber <= newBook.NumberOfPages)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool AddLog(ReadLog newLog)
        {
            var logValidated = validateLog(newLog);

            if (logValidated)
            {
                dbContext.Add(newLog);
                dbContext.SaveChanges();
            }

            return logValidated;
        }

        public void DeleteLog(int logId)
        {
            var logToDelete = dbContext.ReadLogs.Single(l => l.ReadLogId == logId);
            dbContext.Remove(logToDelete);
            dbContext.SaveChanges();
        }

        public ReadLog GetLogById(int logId)
        {
            return dbContext.ReadLogs.Include(l => l.Book).SingleOrDefault(l => l.ReadLogId == logId);
        }

        public bool UpdateLog(ReadLog log)
        {
            var logValidated = validateLog(log);

            if (logValidated)
            {
                dbContext.Update(log);
                dbContext.SaveChanges();
            }

            return logValidated;
        }
    }
}
