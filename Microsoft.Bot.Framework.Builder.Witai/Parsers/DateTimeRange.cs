using System;

namespace Microsoft.Bot.Framework.Builder.Witai.Parsers
{
    [Serializable]
    public class DateTimeRange
    {
        public DateTimeRange(DateTime date)
        {
            IsSingleDate = true;
            StartDate = date;
            EndDate = date;
        }

        public DateTimeRange(DateTime startDate, DateTime endDate)
        {
            IsSingleDate = false;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsSingleDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
