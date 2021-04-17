using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.DTO
{
  public class FilteringItems
  {
    public IEnumerable<SelectItem> Zones { get; set; }
    public IEnumerable<SelectItem> Types { get; set; }
    public IEnumerable<SelectItem> Directions { get; set; }
    public IEnumerable<SelectItem> NatInts { get; set; }
    public IEnumerable<SelectItem> States { get; set; }
    public IEnumerable<SelectItem> Initiators { get; set; }
    public IEnumerable<SelectItem> TicketPriorities { get; set; }
    public IEnumerable<SelectItem> Categories { get; set; }
    public IEnumerable<SelectItem> ProblemSides { get; set; }
  }
}
