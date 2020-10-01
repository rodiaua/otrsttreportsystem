using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  public class ResetPasswordModel
  {
    [Required]
    public string Id { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
