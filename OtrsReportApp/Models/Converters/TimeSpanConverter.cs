using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Converters
{
  public class TimeSpanConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      long timeSpan = 0;
      switch (reader.Value)
      {
        case int intValue:
          timeSpan = intValue;
          break;
        case long longValue:
          timeSpan = longValue;
          break;
        case string strValue:
          timeSpan = long.Parse(strValue);
          break;
        default:
          return null;
      }
      return TimeSpan.FromSeconds(timeSpan);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var ts = ((TimeSpan)value);
      writer.WriteValue((int)ts.TotalSeconds);
    }
  }
}
