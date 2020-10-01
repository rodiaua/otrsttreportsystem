using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  public class UpdateUserModel
  {
    [Required]
    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
  }
}
