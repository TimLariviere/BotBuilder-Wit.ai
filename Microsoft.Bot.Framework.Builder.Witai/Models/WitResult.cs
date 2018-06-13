using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Bot.Framework.Builder.Witai.Models
{
    public class WitResult
    {
        [JsonProperty(PropertyName = "msg_id")]
        public string MessageId { get; set; }

        [JsonProperty(PropertyName = "_text")]
        public string Text { get; set; }

        public Dictionary<string, IList<WitEntity>> Entities { get; set; }

        public string Error { get; set; }

        public string Code { get; set; }
    }
}
