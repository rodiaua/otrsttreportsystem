﻿using System;
using System.Collections.Generic;

namespace OtrsReportApp.Models
{
    public partial class TicketState
    {
        public TicketState()
        {
            Ticket = new HashSet<Ticket>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public short TypeId { get; set; }
        public short ValidId { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateBy { get; set; }
        public DateTime ChangeTime { get; set; }
        public int ChangeBy { get; set; }

        public virtual Users ChangeByNavigation { get; set; }
        public virtual Users CreateByNavigation { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
