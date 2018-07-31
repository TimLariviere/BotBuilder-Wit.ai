using Microsoft.Bot.Framework.Builder.Witai.Models;
using System;

namespace Microsoft.Bot.Framework.Builder.Witai.Parsers
{
    public static class DateTimeParser
    {
        private const string TypeValue = "value";
        private const string TypeInterval = "interval";
        private const string GrainHour = "hour";
        private const string GrainDay = "day";
        private const string GrainWeek = "week";
        private const string GrainMonth = "month";
        private const string GrainYear = "year";

        public static bool TryParse(WitEntity entity, out DateTimeRange range, DateTimeParserSettings settings = null)
        {
            settings = settings ?? DateTimeParserSettings.Default;

            if (IsDate(entity))
            {
                switch (entity.Type)
                {
                    case TypeValue:
                        return TryParseAsValue(entity, settings, out range);
                    case TypeInterval:
                        return TryParseAsInterval(entity, settings, out range);
                }
            }

            range = null;
            return false;
        }

        private static bool IsDate(WitEntity entity)
        {
            return entity.Type == TypeValue && (entity.Grain == GrainHour || entity.Grain == GrainDay || entity.Grain == GrainWeek || entity.Grain == GrainMonth || entity.Grain == GrainYear)
                   || entity.Type == TypeInterval;
        }

        private static bool TryParseAsValue(WitEntity entity, DateTimeParserSettings settings, out DateTimeRange range)
        {
            if (entity != null && DateTimeOffset.TryParse(entity.Value, out DateTimeOffset date))
            {
                switch (entity.Grain)
                {
                    case GrainHour:
                        range = new DateTimeRange(date, date.AddHours(1));
                        return true;
                    case GrainDay:
                        range = new DateTimeRange(date);
                        return true;
                    case GrainWeek:
                        range = new DateTimeRange(date, date.AddDays(settings.WeekDuration).AddSeconds(-1));
                        return true;
                    case GrainMonth:
                        range = new DateTimeRange(date, date.AddMonths(1).AddSeconds(-1));
                        return true;
                    case GrainYear:
                        range = new DateTimeRange(date, date.AddYears(1).AddSeconds(-1));
                        return true;
                }
            }

            range = null;
            return false;
        }

        private static bool TryParseAsInterval(WitEntity entity, DateTimeParserSettings settings, out DateTimeRange range)
        {
            TryParseAsValue(entity.From, settings, out DateTimeRange fromRange);
            TryParseAsValue(entity.To, settings, out DateTimeRange toRange);
            if (fromRange != null && toRange != null)
            {
                range = new DateTimeRange(fromRange.StartDate, toRange.EndDate);
                return true;
            }
            else if (fromRange != null)
            {
                range = new DateTimeRange(fromRange.StartDate, DateTime.MaxValue);
                return true;
            }
            else if (toRange != null)
            {
                range = new DateTimeRange(DateTime.Now, toRange.EndDate);
                return true;
            }

            range = null;
            return false;
        }
    }

    public class DateTimeParserSettings
    {
        public static DateTimeParserSettings Default = new DateTimeParserSettings { WeekDuration = 5 };

        public int WeekDuration { get; set; } = 5;
    }
}
