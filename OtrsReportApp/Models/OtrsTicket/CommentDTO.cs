using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class CommentDTO
  {
    public long PendedTicketId { get; set; }
    public int? CommentId { get; set; }
    public string CommentText { get; set; }
  }
}
