using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Models.Converters
{
  public class EpochConverter : JsonConverter
  {
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      long epochTime = 0;
      switch(reader.Value)
      {
        case int intValue:
          epochTime = intValue;
          break;
        case long longValue:
          epochTime = longValue;
          break;
        case string strValue:
          epochTime = long.Parse(strValue);
          break;
        default:
          return null;
      }
      return _epoch.AddSeconds(epochTime);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var utcValue = ((DateTime)value);
      writer.WriteValue(((int)(utcValue - _epoch).TotalSeconds));
    }
  }
}
