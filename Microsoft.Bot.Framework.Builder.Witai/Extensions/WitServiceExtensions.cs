using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai.Extensions
{
    public static class WitServiceExtensions
    {
        public static async Task<WitResult> QueryAsync(this IWitService service, IDialogContext context, string text, string threadId, string ctx, CancellationToken token)
        {
            return await service.QueryAsync(context, new WitRequest(text, threadId, ctx), token);
        }
    }
}
