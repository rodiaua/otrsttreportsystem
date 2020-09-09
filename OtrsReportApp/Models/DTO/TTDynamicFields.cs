using OtrsReportApp.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace OtrsReportApp.Models
{
  public class TTDynamicFields
  {
    public TicketDTO Ticket { get; set; }
    public IEnumerable<string> DynamicFields { get; set; }
  }
}
