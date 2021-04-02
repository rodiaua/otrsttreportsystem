using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.DTO
{
  public class Filters
  {
    public Period Period { get; set; }
    public string[] Zones { get; set; }
    public string[] Types { get; set; }
    public string[] Initiators { get; set; }
    public string[] Directions { get; set; }
    public string[] NatInts { get; set; }
    public string[] Priorities { get; set; }
    public string[] States { get; set; }
    public string[] Categories { get; set; }
  }
}
