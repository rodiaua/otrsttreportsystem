using Microsoft.AspNetCore.SignalR;
using OtrsReportApp.Models.Logging;
using OtrsReportApp.Services.LoggingHubService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data.Logging
{
  public class SqlLoggingDatabase : ILoggingDatabase
  {
    private readonly LoggingDbContext _context;
    private readonly IHubContext<LoggingHub, ILoggingHubService> _hubContext;
    public SqlLoggingDatabase(LoggingDbContext context, IHubContext<LoggingHub, ILoggingHubService> hubContext)
    {
      _context = context;
      _hubContext = hubContext;
    }
    public async Task Save(LogItem item)
    {
      _context.Add(item);
      await _context.SaveChangesAsync();
      item.Time = item.Time.ToLocalTime();
      await _hubContext.Clients.All.GetLog(new[] { item }.ToList());
    }

    public async Task SaveRange(List<LogItem> items)
    {
      _context.AddRange(items);
      await _context.SaveChangesAsync();
      items.ForEach(item => { item.Time = item.Time.ToLocalTime(); });
      await _hubContext.Clients.All.GetLog(items);
    }
  }
}
