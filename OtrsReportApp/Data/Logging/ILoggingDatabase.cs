using OtrsReportApp.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data.Logging
{
  public interface ILoggingDatabase
  {
    public Task Save(LogItem item);
    public Task SaveRange(List<LogItem> items);
  }
}
