using Microsoft.Bot.Framework.Builder.Witai.Models;
using System.Collections.Generic;
using System.Linq;

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

        public static bool TryFindEntities(this WitResult result, string entityName, out IEnumerable<WitEntity> witEntities)
        {
            var entity = result?.Entities?.FirstOrDefault(e => e.Key == entityName);
            witEntities = entity?.Value;
            return witEntities != null;
        }
    }
}
