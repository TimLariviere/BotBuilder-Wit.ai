using System;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai.Parsers
{
    public static class DateTimeParser
    {
        public static DateTimeRange Parse(WitEntity entity)
        {
            if (entity.Grain == "day")
            {
                var dateStr = entity.Value.Remove(entity.Value.IndexOf("T"));
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    return new DateTimeRange(date.Date);
                }
            }
            else if (entity.Type == "interval")
            {
                return new DateTimeRange(Parse(entity.From).StartDate, Parse(entity.To).EndDate.AddDays(-1));
            }

            return null;
        }
    }
}
