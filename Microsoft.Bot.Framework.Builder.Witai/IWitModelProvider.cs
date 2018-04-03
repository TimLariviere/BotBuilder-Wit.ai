using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitModelProvider
    {
        Task<IWitModel> GetWitModelAsync(IDialogContext context);
    }
}

