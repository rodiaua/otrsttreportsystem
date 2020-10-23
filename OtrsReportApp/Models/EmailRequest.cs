using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models
{
  public class EmailRequest
  {
    public string To{ get; set; }
    public string Body { get; set; }
    public string Subject { get; set; }
  }
}
