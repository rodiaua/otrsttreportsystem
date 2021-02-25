using OtrsReportApp.Models.Account;
using OtrsReportApp.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data.Logging
{
  public class Logger
  {
    private readonly ILoggingDatabase _database;

    public AccountUser User { get; set; }

    public Logger(ILoggingDatabase database)
    {
      _database = database;
    }

    public Task Log(string content)
    {

      LogItem item = new LogItem();
      if(User != null)
      {
        item.FirstName = User.FirstName;
        item.LastName = User.LastName;
        item.UserName = User.UserName;
      }
      item.Content = content;
      return _database.Save(item);
    }

    public Task LogRange(List<string> contents)
    { 
      List<LogItem> logItems = new();
      contents.ForEach(x => {
        LogItem item = new LogItem();
        if (User != null)
        {
          item.FirstName = User.FirstName;
          item.LastName = User.LastName;
          item.UserName = User.UserName;
        }
        item.Content = x;
      });
      
      return _database.SaveRange(logItems);
    }
  }
}
