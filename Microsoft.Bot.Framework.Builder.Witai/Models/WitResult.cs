using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Framework.Builder.Witai.Models
{
    public class WitResult
    {
        [JsonProperty(PropertyName = "msg_id")]
        public Guid MessageId { get; set; }

        [JsonProperty(PropertyName = "_text")]
        public string Text { get; set; }

        public Dictionary<string, IList<WitEntity>> Entities { get; set; }
    }
}
