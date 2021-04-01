using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public interface IReadLogsRepository
    {
        bool AddLog(ReadLog newLog);

        void DeleteLog(int logId);

        ReadLog GetLogById(int logId);

        bool UpdateLog(ReadLog log);
    }
}
