using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using System;

namespace Microsoft.Bot.Framework.Builder.Witai.Dialogs
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [Serializable]
    public class WitActionAttribute : AttributeString
    {
        public readonly string ActionName;

        public WitActionAttribute(string actionName)
        {
            SetField.NotNull(out this.ActionName, nameof(actionName), actionName);
        }

        protected override string Text
        {
            get
            {
                return this.ActionName;
            }
        }
    }
}
