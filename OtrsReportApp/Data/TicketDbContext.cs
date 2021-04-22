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

    public virtual DbSet<TicketComment> TicketComment { get; set; }
    public virtual DbSet<PendingTicketRestrictedQueue> PendingTicketRestrictedQueue { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
      base.OnModelCreating(builder);

      builder.Entity<AcknowledgedTicket>().ToTable("acknowledged_ticket").HasKey(t => t.Id);
      builder.Entity<PendedTicket>(entity => {
        entity.HasKey("Id", "TicketId");
        entity.HasIndex(pt => pt.TicketId).IsUnique();
        entity.ToTable("pended_ticket");
      });
      builder.Entity<TicketComment>().ToTable("ticket_comment");
      builder.Entity<PendingTicketRestrictedQueue>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.QueueId).IsUnique();
        entity.ToTable("pending_ticket_restricted_queue");
        entity.HasData(
          new PendingTicketRestrictedQueue { Name = "test_But", QueueId = 32,Id =1  },
          new PendingTicketRestrictedQueue { Name = "testing_vas", QueueId = 38, Id=2 },
          new PendingTicketRestrictedQueue { Name = "Testing_operators", QueueId = 39, Id =3 },
          new PendingTicketRestrictedQueue { Name = "TestSurvey", QueueId = 42, Id=4 },
          new PendingTicketRestrictedQueue { Name = "daily testing", QueueId = 52,Id=5 }
          );
      });
    }
  }
}
