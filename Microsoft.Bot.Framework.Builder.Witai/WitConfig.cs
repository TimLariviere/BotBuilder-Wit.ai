using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public class WitConfig : IWitConfig
    {
        public Dictionary<CultureInfo, string> WitConfigDictionary { get; set; }
    }
}
