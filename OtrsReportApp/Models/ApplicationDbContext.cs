using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace OtrsReportApp.Models
{
  public partial class ApplicationDbContext : DbContext
  {
    private readonly ILoggerFactory _loggerFactory;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory loggerFactory)
        : base(options)
    {
      _loggerFactory = loggerFactory;
    }

    public virtual DbSet<Article> Article { get; set; }
    public virtual DbSet<DynamicField> DynamicField { get; set; }
    public virtual DbSet<DynamicFieldValue> DynamicFieldValue { get; set; }
    public virtual DbSet<Queue> Queue { get; set; }
    public virtual DbSet<Ticket> Ticket { get; set; }
    public virtual DbSet<TicketPriority> TicketPriority { get; set; }
    public virtual DbSet<TicketState> TicketState { get; set; }
    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      //optionsBuilder.UseLoggerFactory(_loggerFactory)
      //  .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Article>(entity =>
      {
        entity.ToTable("article");

        entity.HasIndex(e => e.AMessageIdMd5)
                  .HasName("article_message_id_md5");

        entity.HasIndex(e => e.ArticleSenderTypeId)
                  .HasName("article_article_sender_type_id");

        entity.HasIndex(e => e.ArticleTypeId)
                  .HasName("article_article_type_id");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_article_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_article_create_by_id");

        entity.HasIndex(e => e.TicketId)
                  .HasName("article_ticket_id");

        entity.HasIndex(e => e.ValidId)
                  .HasName("FK_article_valid_id_id");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.ABody)
                  .IsRequired()
                  .HasColumnName("a_body")
                  .HasColumnType("mediumtext")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ACc)
                  .HasColumnName("a_cc")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AContentType)
                  .HasColumnName("a_content_type")
                  .HasColumnType("varchar(250)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AFrom)
                  .HasColumnName("a_from")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AInReplyTo)
                  .HasColumnName("a_in_reply_to")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AMessageId)
                  .HasColumnName("a_message_id")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AMessageIdMd5)
                  .HasColumnName("a_message_id_md5")
                  .HasColumnType("varchar(32)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AReferences)
                  .HasColumnName("a_references")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.AReplyTo)
                  .HasColumnName("a_reply_to")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ASubject)
                  .HasColumnName("a_subject")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ATo)
                  .HasColumnName("a_to")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ArticleSenderTypeId)
                  .HasColumnName("article_sender_type_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ArticleTypeId)
                  .HasColumnName("article_type_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.ContentPath)
                  .HasColumnName("content_path")
                  .HasColumnType("varchar(250)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.IncomingTime)
                  .HasColumnName("incoming_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.TicketId)
                  .HasColumnName("ticket_id")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.ArticleChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_article_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.ArticleCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_article_create_by_id");

        entity.HasOne(d => d.Ticket)
                  .WithMany(p => p.Article)
                  .HasForeignKey(d => d.TicketId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_article_ticket_id_id");
      });

      modelBuilder.Entity<DynamicField>(entity =>
      {
        entity.ToTable("dynamic_field");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_dynamic_field_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_dynamic_field_create_by_id");

        entity.HasIndex(e => e.Name)
                  .HasName("dynamic_field_name")
                  .IsUnique();

        entity.HasIndex(e => e.ValidId)
                  .HasName("FK_dynamic_field_valid_id_id");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.Config).HasColumnName("config");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.FieldOrder)
                  .HasColumnName("field_order")
                  .HasColumnType("int(11)");

        entity.Property(e => e.FieldType)
                  .IsRequired()
                  .HasColumnName("field_type")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.InternalField)
                  .HasColumnName("internal_field")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.Label)
                  .IsRequired()
                  .HasColumnName("label")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasColumnName("name")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ObjectType)
                  .IsRequired()
                  .HasColumnName("object_type")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.DynamicFieldChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_dynamic_field_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.DynamicFieldCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_dynamic_field_create_by_id");
      });

      modelBuilder.Entity<DynamicFieldValue>(entity =>
      {
        entity.ToTable("dynamic_field_value");

        entity.HasIndex(e => new { e.FieldId, e.ValueDate })
                  .HasName("dynamic_field_value_search_date");

        entity.HasIndex(e => new { e.FieldId, e.ValueInt })
                  .HasName("dynamic_field_value_search_int");

        entity.HasIndex(e => new { e.ObjectId, e.FieldId })
                  .HasName("dynamic_field_value_field_values");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.FieldId)
                  .HasColumnName("field_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ObjectId)
                  .HasColumnName("object_id")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.ValueDate).HasColumnName("value_date");

        entity.Property(e => e.ValueInt)
                  .HasColumnName("value_int")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.ValueText)
                  .HasColumnName("value_text")
                  .HasColumnType("text")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.HasOne(d => d.Field)
                  .WithMany(p => p.DynamicFieldValue)
                  .HasForeignKey(d => d.FieldId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_dynamic_field_value_field_id_id");
      });

      modelBuilder.Entity<Queue>(entity =>
      {
        entity.ToTable("queue");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_queue_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_queue_create_by_id");

        entity.HasIndex(e => e.FollowUpId)
                  .HasName("FK_queue_follow_up_id_id");

        entity.HasIndex(e => e.GroupId)
                  .HasName("queue_group_id");

        entity.HasIndex(e => e.Name)
                  .HasName("queue_name")
                  .IsUnique();

        entity.HasIndex(e => e.SalutationId)
                  .HasName("FK_queue_salutation_id_id");

        entity.HasIndex(e => e.SignatureId)
                  .HasName("FK_queue_signature_id_id");

        entity.HasIndex(e => e.SystemAddressId)
                  .HasName("FK_queue_system_address_id_id");

        entity.HasIndex(e => e.ValidId)
                  .HasName("FK_queue_valid_id_id");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CalendarName)
                  .HasColumnName("calendar_name")
                  .HasColumnType("varchar(100)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.Comments)
                  .HasColumnName("comments")
                  .HasColumnType("varchar(250)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.DefaultSignKey)
                  .HasColumnName("default_sign_key")
                  .HasColumnType("varchar(100)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.FirstResponseNotify)
                  .HasColumnName("first_response_notify")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.FirstResponseTime)
                  .HasColumnName("first_response_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.FollowUpId)
                  .HasColumnName("follow_up_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.FollowUpLock)
                  .HasColumnName("follow_up_lock")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.GroupId)
                  .HasColumnName("group_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasColumnName("name")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.SalutationId)
                  .HasColumnName("salutation_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.SignatureId)
                  .HasColumnName("signature_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.SolutionNotify)
                  .HasColumnName("solution_notify")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.SolutionTime)
                  .HasColumnName("solution_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.SystemAddressId)
                  .HasColumnName("system_address_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.UnlockTimeout)
                  .HasColumnName("unlock_timeout")
                  .HasColumnType("int(11)");

        entity.Property(e => e.UpdateNotify)
                  .HasColumnName("update_notify")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.UpdateTime)
                  .HasColumnName("update_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.QueueChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_queue_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.QueueCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_queue_create_by_id");
      });

      modelBuilder.Entity<Ticket>(entity =>
      {
        entity.ToTable("ticket");

        entity.HasIndex(e => e.ArchiveFlag)
                  .HasName("ticket_archive_flag");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_ticket_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_ticket_create_by_id");

        entity.HasIndex(e => e.CreateTime)
                  .HasName("ticket_create_time");

        entity.HasIndex(e => e.CreateTimeUnix)
                  .HasName("ticket_create_time_unix");

        entity.HasIndex(e => e.CustomerId)
                  .HasName("ticket_customer_id");

        entity.HasIndex(e => e.CustomerUserId)
                  .HasName("ticket_customer_user_id");

        entity.HasIndex(e => e.EscalationResponseTime)
                  .HasName("ticket_escalation_response_time");

        entity.HasIndex(e => e.EscalationSolutionTime)
                  .HasName("ticket_escalation_solution_time");

        entity.HasIndex(e => e.EscalationTime)
                  .HasName("ticket_escalation_time");

        entity.HasIndex(e => e.EscalationUpdateTime)
                  .HasName("ticket_escalation_update_time");

        entity.HasIndex(e => e.QueueId)
                  .HasName("ticket_queue_id");

        entity.HasIndex(e => e.ResponsibleUserId)
                  .HasName("ticket_responsible_user_id");

        entity.HasIndex(e => e.ServiceId)
                  .HasName("FK_ticket_service_id_id");

        entity.HasIndex(e => e.SlaId)
                  .HasName("FK_ticket_sla_id_id");

        entity.HasIndex(e => e.TicketLockId)
                  .HasName("ticket_ticket_lock_id");

        entity.HasIndex(e => e.TicketPriorityId)
                  .HasName("ticket_ticket_priority_id");

        entity.HasIndex(e => e.TicketStateId)
                  .HasName("ticket_ticket_state_id");

        entity.HasIndex(e => e.Timeout)
                  .HasName("ticket_timeout");

        entity.HasIndex(e => e.Title)
                  .HasName("ticket_title");

        entity.HasIndex(e => e.Tn)
                  .HasName("ticket_tn")
                  .IsUnique();

        entity.HasIndex(e => e.TypeId)
                  .HasName("ticket_type_id");

        entity.HasIndex(e => e.UntilTime)
                  .HasName("ticket_until_time");

        entity.HasIndex(e => e.UserId)
                  .HasName("ticket_user_id");

        entity.HasIndex(e => new { e.TicketStateId, e.TicketLockId })
                  .HasName("ticket_queue_view");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.ArchiveFlag)
                  .HasColumnName("archive_flag")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.CreateTimeUnix)
                  .HasColumnName("create_time_unix")
                  .HasColumnType("bigint(20)");

        entity.Property(e => e.CustomerId)
                  .HasColumnName("customer_id")
                  .HasColumnType("varchar(150)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.CustomerUserId)
                  .HasColumnName("customer_user_id")
                  .HasColumnType("varchar(250)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.EscalationResponseTime)
                  .HasColumnName("escalation_response_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.EscalationSolutionTime)
                  .HasColumnName("escalation_solution_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.EscalationTime)
                  .HasColumnName("escalation_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.EscalationUpdateTime)
                  .HasColumnName("escalation_update_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.QueueId)
                  .HasColumnName("queue_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ResponsibleUserId)
                  .HasColumnName("responsible_user_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ServiceId)
                  .HasColumnName("service_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.SlaId)
                  .HasColumnName("sla_id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.TicketLockId)
                  .HasColumnName("ticket_lock_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.TicketPriorityId)
                  .HasColumnName("ticket_priority_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.TicketStateId)
                  .HasColumnName("ticket_state_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.Timeout)
                  .HasColumnName("timeout")
                  .HasColumnType("int(11)");

        entity.Property(e => e.Title)
                  .HasColumnName("title")
                  .HasColumnType("varchar(255)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.Tn)
                  .IsRequired()
                  .HasColumnName("tn")
                  .HasColumnType("varchar(50)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.TypeId)
                  .HasColumnName("type_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.UntilTime)
                  .HasColumnName("until_time")
                  .HasColumnType("int(11)");

        entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .HasColumnType("int(11)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.TicketChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.TicketCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_create_by_id");

        entity.HasOne(d => d.Queue)
                  .WithMany(p => p.Ticket)
                  .HasForeignKey(d => d.QueueId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_queue_id_id");

        entity.HasOne(d => d.ResponsibleUser)
                  .WithMany(p => p.TicketResponsibleUser)
                  .HasForeignKey(d => d.ResponsibleUserId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_responsible_user_id_id");

        entity.HasOne(d => d.TicketPriority)
                  .WithMany(p => p.Ticket)
                  .HasForeignKey(d => d.TicketPriorityId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_ticket_priority_id_id");

        entity.HasOne(d => d.TicketState)
                  .WithMany(p => p.Ticket)
                  .HasForeignKey(d => d.TicketStateId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_ticket_state_id_id");

        entity.HasOne(d => d.User)
                  .WithMany(p => p.TicketUser)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_user_id_id");
      });

      modelBuilder.Entity<TicketPriority>(entity =>
      {
        entity.ToTable("ticket_priority");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_ticket_priority_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_ticket_priority_create_by_id");

        entity.HasIndex(e => e.Name)
                  .HasName("ticket_priority_name")
                  .IsUnique();

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasColumnName("name")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.TicketPriorityChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_priority_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.TicketPriorityCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_priority_create_by_id");
      });

      modelBuilder.Entity<TicketState>(entity =>
      {
        entity.ToTable("ticket_state");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_ticket_state_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_ticket_state_create_by_id");

        entity.HasIndex(e => e.Name)
                  .HasName("ticket_state_name")
                  .IsUnique();

        entity.HasIndex(e => e.TypeId)
                  .HasName("FK_ticket_state_type_id_id");

        entity.HasIndex(e => e.ValidId)
                  .HasName("FK_ticket_state_valid_id_id");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.Comments)
                  .HasColumnName("comments")
                  .HasColumnType("varchar(250)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.Name)
                  .IsRequired()
                  .HasColumnName("name")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.TypeId)
                  .HasColumnName("type_id")
                  .HasColumnType("smallint(6)");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.TicketStateChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_state_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.TicketStateCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ticket_state_create_by_id");
      });

      modelBuilder.Entity<Users>(entity =>
      {
        entity.ToTable("users");

        entity.HasIndex(e => e.ChangeBy)
                  .HasName("FK_users_change_by_id");

        entity.HasIndex(e => e.CreateBy)
                  .HasName("FK_users_create_by_id");

        entity.HasIndex(e => e.Login)
                  .HasName("users_login")
                  .IsUnique();

        entity.HasIndex(e => e.ValidId)
                  .HasName("FK_users_valid_id_id");

        entity.Property(e => e.Id)
                  .HasColumnName("id")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeBy)
                  .HasColumnName("change_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.ChangeTime).HasColumnName("change_time");

        entity.Property(e => e.CreateBy)
                  .HasColumnName("create_by")
                  .HasColumnType("int(11)");

        entity.Property(e => e.CreateTime).HasColumnName("create_time");

        entity.Property(e => e.FirstName)
                  .IsRequired()
                  .HasColumnName("first_name")
                  .HasColumnType("varchar(100)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.LastName)
                  .IsRequired()
                  .HasColumnName("last_name")
                  .HasColumnType("varchar(100)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.Login)
                  .IsRequired()
                  .HasColumnName("login")
                  .HasColumnType("varchar(200)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.Pw)
                  .IsRequired()
                  .HasColumnName("pw")
                  .HasColumnType("varchar(64)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.Title)
                  .HasColumnName("title")
                  .HasColumnType("varchar(50)")
                  .HasCharSet("utf8")
                  .HasCollation("utf8_general_ci");

        entity.Property(e => e.ValidId)
                  .HasColumnName("valid_id")
                  .HasColumnType("smallint(6)");

        entity.HasOne(d => d.ChangeByNavigation)
                  .WithMany(p => p.InverseChangeByNavigation)
                  .HasForeignKey(d => d.ChangeBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_users_change_by_id");

        entity.HasOne(d => d.CreateByNavigation)
                  .WithMany(p => p.InverseCreateByNavigation)
                  .HasForeignKey(d => d.CreateBy)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_users_create_by_id");
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
