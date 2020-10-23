using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  public class TwoFaResult
  {
    public bool Succeeded { get; set; }
    public string Error{ get; set; }
    public string Token { get; set; }

  }
}
