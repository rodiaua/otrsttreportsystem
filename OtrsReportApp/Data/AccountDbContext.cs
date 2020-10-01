using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OtrsReportApp.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data
{
  public class AccountDbContext : IdentityDbContext<AccountUser, AccountRole, string>
  {
    public AccountDbContext(DbContextOptions options) : base(options)
    {

    }


  }
}
