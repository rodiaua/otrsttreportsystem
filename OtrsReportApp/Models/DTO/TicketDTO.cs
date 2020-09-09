using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.DTO
{
  public class TicketDTO
  {
    public long Id { get; set; }

    public string Tn { get; set; }

    public string Title { get; set; }

    public int QueueId { get; set; }

    public virtual TicketStateDTO TicketState { get; set; }

    public virtual QueueDTO Queue { get; set; }

    public DateTime CreateTime { get; set; }
  }
}
