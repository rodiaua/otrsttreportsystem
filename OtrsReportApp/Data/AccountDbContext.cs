using Microsoft.AspNetCore.Identity;
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<AccountUser>(b =>
      {
        b.ToTable("aspnetusers");
      });
      builder.Entity<AccountRole>(b =>
      {
        b.ToTable("aspnetroles");
      });
      builder.Entity<IdentityUserRole<string>>()
        .ToTable("aspnetuserroles");
    }
  }
}
