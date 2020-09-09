using OtrsReportApp.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Extensions
{
  public static class To
  {
    public static IEnumerable<SelectItem> toSelectItems(this IEnumerable<string> collection)
    {
      foreach(var item in collection)
      {
        yield return new SelectItem() { Label = item, Value = item };
      }
    }
  }
}
