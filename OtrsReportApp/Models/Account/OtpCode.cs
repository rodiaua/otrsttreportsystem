using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  public class OtpCode
  {
    public int Id { get; set; }
    public string Code { get; set; }
    public string UserId { get; set; }
    public DateTime Expires { get; set; }
  }
}
