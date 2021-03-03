using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class AcknowledgedTicket
  {
    public int Id { get; set; }
    public long TicketId { get; set; }
    [JsonConverter(typeof(EpochConverter))]
    public DateTime CreateTime { get; set; }
  }
}
