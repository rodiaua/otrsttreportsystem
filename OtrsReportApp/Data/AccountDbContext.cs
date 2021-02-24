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
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {

    }

    public DbSet<OtpCode> OtpCodes { get; set; }

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
      builder.Entity<OtpCode>(m =>
      {
        m.ToTable("otpcodes");
        m.HasKey(r => r.Id);
        m.Property(r => r.UserId).IsRequired();
        m.Property(r => r.Code).IsRequired();
        m.Property(r => r.Expires).IsRequired();
      });
    }
  }
}
