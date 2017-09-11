using System.Linq;
using Microsoft.Bot.Framework.Builder.Witai.Models;

namespace Microsoft.Bot.Framework.Builder.Witai.Extensions
{
    public static class WitResultExtensions
    {
        public static bool TryFindEntity(this WitResult result, string entityName, out WitEntity witEntity)
        {
            var entity = result?.Entities?.FirstOrDefault(e => e.Key == entityName);
            witEntity = entity?.Value?.FirstOrDefault();
            return witEntity != null;
        }
    }
}
