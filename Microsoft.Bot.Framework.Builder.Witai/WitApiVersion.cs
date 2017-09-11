using System;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public enum WitApiVersionType
    {
        Latest,
        Custom
    }

    [Serializable]
    public class WitApiVersion
    {
        public static readonly WitApiVersion Latest = new WitApiVersion();
        public static WitApiVersion Custom(string version) => new WitApiVersion(version);

        private string _customValue;

        private WitApiVersion()
        {
            Type = WitApiVersionType.Latest;
        }

        private WitApiVersion(string customValue)
        {
            Type = WitApiVersionType.Custom;
            SetField.NotNull(out _customValue, nameof(customValue), customValue);
        }

        public WitApiVersionType Type { get; }
        public string CustomValue => _customValue;
    }
}
