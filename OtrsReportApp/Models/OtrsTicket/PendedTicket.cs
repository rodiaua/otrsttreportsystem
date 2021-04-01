using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class PendedTicket
  {
    public int Id { get; set; }
    public long TicketId { get; set; }
    public int Overdue{ get; set; }
  }
}
