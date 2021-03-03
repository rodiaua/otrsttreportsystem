using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using OtrsReportApp.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data
{
  public class LoggingDbContext : DbContext
  {
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
    {
      
    }

    public virtual DbSet<LogItem> LogItem { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<LogItem>().ToTable("log").HasKey(t => t.Id);

    }
  }
}
