using OtrsReportApp.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services.LoggingHubService
{
  public interface ILoggingHubService
  {
    Task GetLog(List<LogItem> logItems);
  }
}
