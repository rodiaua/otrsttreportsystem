using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Services.LoggingHubService
{
  public class LoggingHub : Hub<ILoggingHubService>
  {
  }
}
