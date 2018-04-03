using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitService
    {
        Task<WitResult> QueryAsync(IDialogContext context, IWitRequest request, CancellationToken token);

        HttpRequestMessage BuildRequest(IWitRequest witRequest, IWitModel model);
    }
}
