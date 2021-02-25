using OtrsReportApp.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data.Logging
{
  public class SqlLoggingDatabase : ILoggingDatabase
  {
    private readonly LoggingDbContext _context;
    public SqlLoggingDatabase(LoggingDbContext context)
    {
      _context = context;
    }
    public async Task Save(LogItem item)
    {
      _context.Add(item);
      await _context.SaveChangesAsync();
    }

    public async Task SaveRange(List<LogItem> items)
    {
      _context.AddRange(items);
      await _context.SaveChangesAsync();
    }
  }
}
