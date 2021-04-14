using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.OtrsTicket
{
  public class OtrsTicketDTO
  {
    public long Id { get; set; }
    public string Tn { get; set; }
    public string Title { get; set; }
    /*[JsonConverter(typeof(EpochConverter))]*/
    public DateTime CreateTime { get; set; }
    public string NatInt { get; set; }
  }
}
