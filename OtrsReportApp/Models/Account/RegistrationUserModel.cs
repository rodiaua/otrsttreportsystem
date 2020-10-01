using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  public class RegistrationUserModel
  {
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
    [Required]
    public string Role { get; set; }
  }
}
