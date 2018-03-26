using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitConfig
    {
        Dictionary<CultureInfo, string> WitConfigDictionary { get; set; }
    }
}
