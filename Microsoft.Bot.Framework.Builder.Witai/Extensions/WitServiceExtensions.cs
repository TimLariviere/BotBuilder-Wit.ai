using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai.Extensions
{
    public static class WitServiceExtensions
    {
        public static async Task<WitResult> QueryAsync(this IWitService service, string text, string threadId, string context, CancellationToken token)
        {
            return await service.QueryAsync(new WitRequest(text, threadId, context), token);
        }
    }
}
