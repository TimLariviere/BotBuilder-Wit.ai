using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public enum WitApiVersionType
    {
        Latest,
        Custom
    }

    public class WitApiVersion
    {
        public static readonly WitApiVersion Latest = new WitApiVersion();
        public static WitApiVersion Custom(string version) => new WitApiVersion(version);

        public WitApiVersionType Type { get; }

        public string CustomValue => customValue;
        private string customValue;

        private WitApiVersion()
        {
            this.Type = WitApiVersionType.Latest;
        }

        private WitApiVersion(string customValue)
        {
            this.Type = WitApiVersionType.Custom;
            SetField.NotNull(out this.customValue, nameof(customValue), customValue);
        }
    }
}
