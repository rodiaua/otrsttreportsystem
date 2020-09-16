using OtrsReportApp.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models
{
  public class Report
  {
    public IEnumerable<TicketReportDTO> ticketReportDTOs { get; set; }

    public FilteringItems filteringItems{ get; set; }
  }
}
