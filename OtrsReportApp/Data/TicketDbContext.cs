using Microsoft.EntityFrameworkCore;
using OtrsReportApp.Models.OtrsTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Data
{
  public class TicketDbContext : DbContext
  {
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }

    public virtual DbSet<AcknowledgedTicket> AcknowledgedTicket { get; set; }

    public virtual DbSet<PendedTicket> PendedTicket { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
      base.OnModelCreating(builder);

      builder.Entity<AcknowledgedTicket>().ToTable("acknowledged_ticket").HasKey(t => t.Id);
      builder.Entity<PendedTicket>().ToTable("pended_ticket").HasKey(t => t.Id);
    }
  }
}
