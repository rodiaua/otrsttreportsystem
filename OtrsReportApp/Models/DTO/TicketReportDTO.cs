using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.DTO
{
  public class TicketReportDTO
  {
    public string TN { get; set; }
    [JsonConverter(typeof(EpochConverter))]
    public DateTime CreateTime { get; set; }
    public string Client { get; set; }
    public string ProblemSide { get; set; }
    public string Zone { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Direction { get; set; }
    public string NatInt { get; set; }
    public string Category { get; set; }
    public string State { get; set; }
    public string Initiator { get; set; }
    public string TicketPriority { get; set; }
    [JsonConverter(typeof(EpochConverter))]
    public DateTime? CloseTime { get; set; }
  }
}
