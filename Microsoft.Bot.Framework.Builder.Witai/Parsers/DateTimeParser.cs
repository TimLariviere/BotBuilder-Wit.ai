using System;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai.Parsers
{
    public static class DateTimeParser
    {
        private const string TypeValue = "value";
        private const string TypeInterval = "interval";
        private const string GrainDay = "day";
        private const string GrainMonth = "month";
        private const string GrainYear = "year";

        public static bool TryParse(WitEntity entity, out DateTimeRange range)
        {
            if (IsDate(entity))
            {
                switch (entity.Type)
                {
                    case TypeValue:
                        return TryParseAsValue(entity, out range);
                    case TypeInterval:
                        return TryParseAsInterval(entity, out range);
                }
            }

            range = null;
            return false;
        }

        private static bool IsDate(WitEntity entity)
        {
            return entity.Type == TypeValue && (entity.Grain == GrainDay || entity.Grain == GrainMonth || entity.Grain == GrainYear)
                   || entity.Type == TypeInterval;
        }

        private static bool TryParseAsValue(WitEntity entity, out DateTimeRange range)
        {
            var dateStr = entity.Value.Remove(entity.Value.IndexOf("T"));

            if (DateTime.TryParse(dateStr, out DateTime date))
            {
                switch (entity.Grain)
                {
                    case GrainDay:
                        range = new DateTimeRange(date);
                        return true;
                    case GrainMonth:
                        range = new DateTimeRange(date, date.AddMonths(1));
                        return true;
                    case GrainYear:
                        range = new DateTimeRange(date, date.AddYears(1));
                        return true;
                }
            }

            range = null;
            return false;
        }

        private static bool TryParseAsInterval(WitEntity entity, out DateTimeRange range)
        {
            if (TryParse(entity.From, out DateTimeRange fromRange)
                && TryParse(entity.To, out DateTimeRange toRange))
            {
                range = new DateTimeRange(fromRange.StartDate, toRange.EndDate);
                return true;
            }

            range = null;
            return false;
        }
    }
}
