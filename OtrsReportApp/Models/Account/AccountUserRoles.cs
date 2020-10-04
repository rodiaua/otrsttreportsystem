using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Account
{
  [Table("aspnetuserroles")]
  public class AccountUserRoles: IdentityUserRole<string>
  {
  }
}
