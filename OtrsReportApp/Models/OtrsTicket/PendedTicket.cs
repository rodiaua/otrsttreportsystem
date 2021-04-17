using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class PendedTicket
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public long TicketId { get; set; }
    public int Overdue{ get; set; }
    [ForeignKey("TicketComment")]
    public int? CommentId { get; set; }
    public TicketComment TicketComment { get; set; }
  }
}
