using System;
using System.Collections.Generic;

namespace OtrsReportApp.Models
{
    public partial class Users
    {
        public Users()
        {
            ArticleChangeByNavigation = new HashSet<Article>();
            ArticleCreateByNavigation = new HashSet<Article>();
            DynamicFieldChangeByNavigation = new HashSet<DynamicField>();
            DynamicFieldCreateByNavigation = new HashSet<DynamicField>();
            InverseChangeByNavigation = new HashSet<Users>();
            InverseCreateByNavigation = new HashSet<Users>();
            QueueChangeByNavigation = new HashSet<Queue>();
            QueueCreateByNavigation = new HashSet<Queue>();
            TicketChangeByNavigation = new HashSet<Ticket>();
            TicketCreateByNavigation = new HashSet<Ticket>();
            TicketPriorityChangeByNavigation = new HashSet<TicketPriority>();
            TicketPriorityCreateByNavigation = new HashSet<TicketPriority>();
            TicketResponsibleUser = new HashSet<Ticket>();
            TicketStateChangeByNavigation = new HashSet<TicketState>();
            TicketStateCreateByNavigation = new HashSet<TicketState>();
            TicketUser = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Pw { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short ValidId { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime ChangeTime { get; set; }
        public int ChangeBy { get; set; }

        public virtual Users ChangeByNavigation { get; set; }
        public virtual Users CreateByNavigation { get; set; }
        public virtual ICollection<Article> ArticleChangeByNavigation { get; set; }
        public virtual ICollection<Article> ArticleCreateByNavigation { get; set; }
        public virtual ICollection<DynamicField> DynamicFieldChangeByNavigation { get; set; }
        public virtual ICollection<DynamicField> DynamicFieldCreateByNavigation { get; set; }
        public virtual ICollection<Users> InverseChangeByNavigation { get; set; }
        public virtual ICollection<Users> InverseCreateByNavigation { get; set; }
        public virtual ICollection<Queue> QueueChangeByNavigation { get; set; }
        public virtual ICollection<Queue> QueueCreateByNavigation { get; set; }
        public virtual ICollection<Ticket> TicketChangeByNavigation { get; set; }
        public virtual ICollection<Ticket> TicketCreateByNavigation { get; set; }
        public virtual ICollection<TicketPriority> TicketPriorityChangeByNavigation { get; set; }
        public virtual ICollection<TicketPriority> TicketPriorityCreateByNavigation { get; set; }
        public virtual ICollection<Ticket> TicketResponsibleUser { get; set; }
        public virtual ICollection<TicketState> TicketStateChangeByNavigation { get; set; }
        public virtual ICollection<TicketState> TicketStateCreateByNavigation { get; set; }
        public virtual ICollection<Ticket> TicketUser { get; set; }
    }
}
