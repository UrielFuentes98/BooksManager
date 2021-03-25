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
        public bool AddLog(ReadLog newLog)
        {
            var newBook = dbContext.Books.Include(b => b.ReadLogs).SingleOrDefault(b => b.BookId == newLog.BookId);
            var previousLogs = newBook.ReadLogs.OrderBy(l => l.PageNumber).ToList();

            var validated = false;

            //If no other logs only check page number is between number of pages of book
            if (previousLogs.Count() == 0)
            {
                if (newLog.PageNumber <= newBook.NumberOfPages)
                {
                    validated = true;
                }
            }
            else
            {
                for (var index = 0; index < previousLogs.Count(); index++)
                { 
                    if (newLog.PageNumber < previousLogs[index].PageNumber)
                    {
                        if (index > 0)
                        {
                            //Only check lower bound if it exist.
                            //Log with lower page num has to have earlier or equal date.
                            if (previousLogs[index - 1].LogDate.Date > newLog.LogDate.Date)
                            {
                                break;
                            }
                        }
                        //Check upper bound.
                        //Log with higher page num has to have later or equal date.
                        if (previousLogs[index].LogDate.Date < newLog.LogDate.Date)
                        {
                            break;
                        }

                        //Both checks passed
                        validated = true;
                        break;
                    }

                    //New log has highest page num, check lower bound and number of pages
                    if (index == previousLogs.Count() - 1)
                    {
                        //Log with lower page num has to have earlier or equal date.
                        if (previousLogs[index].LogDate.Date > newLog.LogDate.Date)
                        {
                            break;
                        }

                        if (newLog.PageNumber <= newBook.NumberOfPages)
                        {
                            validated = true;
                        }
                    }
                }
            }

            if (validated)
            {
                dbContext.Add(newLog);
                dbContext.SaveChanges();
            }

            return validated;
        }

        public void DeleteLog(int logId)
        {
            var logToDelete = dbContext.ReadLogs.Single(l => l.ReadLogId == logId);
            dbContext.Remove(logToDelete);
            dbContext.SaveChanges();
        }

        public ReadLog GetLogById(int logId)
        {
            return dbContext.ReadLogs.SingleOrDefault(l => l.ReadLogId == logId);
        }

        public void UpdateLog(ReadLog log)
        {
            dbContext.Update(log);
            dbContext.SaveChanges();
        }
    }
}
