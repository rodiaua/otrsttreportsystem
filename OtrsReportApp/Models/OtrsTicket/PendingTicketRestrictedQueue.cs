using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class PendingTicketRestrictedQueue
  {
    public int Id { get; set; }
    public int QueueId { get; set; }
    public string Name { get; set; }
  }
}
