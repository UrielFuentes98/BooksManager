using BooksManager.Data;
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
        public void AddLog(ReadLog newLog)
        {
            dbContext.Add(newLog);
            dbContext.SaveChanges();
        }
    }
}
