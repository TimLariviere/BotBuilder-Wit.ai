using System;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai.Parsers
{
    public static class DateTimeParser
    {
        private const string TypeValue = "value";
        private const string TypeInterval = "interval";
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
            return entity.Type == TypeValue && (entity.Grain == GrainDay || entity.Grain == GrainMonth || entity.Grain == GrainYear)
                   || entity.Type == TypeInterval;
        }

        private static bool TryParseAsValue(WitEntity entity, DateTimeParserSettings settings, out DateTimeRange range)
        {
            var dateStr = entity.Value.Remove(entity.Value.IndexOf("T"));

            if (DateTime.TryParse(dateStr, out DateTime date))
            {
                switch (entity.Grain)
                {
                    case GrainDay:
                        range = new DateTimeRange(date);
                        return true;
                    case GrainWeek:
                        range = new DateTimeRange(date, date.AddDays(settings.WeekDuration));
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

        private static bool TryParseAsInterval(WitEntity entity, DateTimeParserSettings settings, out DateTimeRange range)
        {
            if (TryParseAsValue(entity.From, settings, out DateTimeRange fromRange)
                && TryParseAsValue(entity.To, settings, out DateTimeRange toRange))
            {
                range = new DateTimeRange(fromRange.StartDate, toRange.EndDate);
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
