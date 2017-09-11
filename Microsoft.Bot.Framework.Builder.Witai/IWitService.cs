using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitService
    {
        Task<WitResult> QueryAsync(IWitRequest request, CancellationToken token);

        HttpRequestMessage BuildRequest(IWitRequest witRequest);
    }
}
