using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitTokenProvider
    {
        Task<string> GetToken();
    }
}
