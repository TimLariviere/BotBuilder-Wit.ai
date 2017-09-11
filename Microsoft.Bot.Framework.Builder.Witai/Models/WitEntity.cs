namespace Microsoft.Bot.Framework.Builder.Witai.Models
{
    public class WitEntity
    {
        public bool? Suggested { get; set; }

        public float Confidence { get; set; }
        
        public string Type { get; set; }

        public string Value { get; set; }

        public WitEntity[] Values { get; set; }

        public string Grain { get; set; }

        #region DateTime Interval

        public WitEntity To { get; set; }

        public WitEntity From { get; set; }

        #endregion
    }
}
