using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using OtrsReportApp.Models.Converters;

namespace OtrsReportApp.Models
{
  public class Period
  {
    [JsonConverter(typeof(EpochConverter))]
    public DateTime startTime { get; set; }
    [JsonConverter(typeof(EpochConverter))]
    public DateTime endTime { get; set; }
  }
}
