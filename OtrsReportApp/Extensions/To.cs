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

    public static IEnumerable<string> SelectValues(this IEnumerable<Dictionary<string, string>> collection, string key)
    {
      foreach(var item in collection)
      {
        foreach(var dict in item)
        {
          if(dict.Key == key)
          {
            yield return dict.Value;
          }
        }
      }
    }
  }
}
