using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Logging
{
  public class LogItem
  {
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Time { get; set; }
    public string Content { get; set; }

    public LogItem()
    {
      Time = DateTime.Now;
    }
  }
}
